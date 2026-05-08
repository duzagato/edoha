# 05 — Objetos expandidos (relacionamentos hidratados em respostas)

> **Dependência:** Idealmente após a **task 04** (modelos padronizados). Sem ela, ainda dá para fazer, mas há risco de retrabalho.
> **Complexidade:** 🟡 Média — várias queries Dapper com `splitOn` ou múltiplos round-trips.

---

## Diagnóstico — onde retornamos só FK

| Entidade / contexto | FK crua | Deveria expor |
|---|---|---|
| `Lottery.IdInstitution` | `Guid` | `Institution Institution` |
| `Ticketbook.IdLottery` | `Guid` | `Lottery Lottery` |
| `Ticketbook.IdStatusTicketbook` | `int` | `StatusTicketbook Status` |
| `Ticketbook.IdOwner` (no DTO de criação; a entidade já tem `TicketbookOwner` aninhado — bom) | — | já hidratado, manter |
| `Ticketbook.IdHolder` | — | já hidratado | 
| `Ticket.IdTicketbook` | `Guid` | manter `Guid` por padrão; expor `Ticketbook` apenas em endpoint que faça sentido |
| `User.Institutions` | já é `List<Institution>` | OK (provém de `SelectUsersAndInstitutions`) |
| `UserInstitution.IdUser` / `IdInstitution` | `Guid`+`Guid` | `User User`, `Institution Institution` |
| `UserPermission.IdUser/IdPage/IdAction/IdPermission` | `Guid`s | `User`, `Page`, `Action`, `Permission` (ao menos `Page.Name` e `Action.Name` que já é projetado em `UserPermissionExpand`) |
| `Login.IdUser` | `Guid` | depende — Login normalmente é interno, não retornar |
| `Page.IdPermission` (se existir) | — | conferir entidade |

> A regra geral pedida: **manter FK** na entidade (necessária para persistência) **e adicionar a navegação** opcional `null` quando o GET hidratar. Um modelo análogo ao Entity Framework: `Guid IdInstitution` + `Institution? Institution`.

---

## Estratégia — duas abordagens, escolher por endpoint

### A. **Hidratação no Service (round-trip extra)**
O service chama o repositório principal, depois itera e chama outro repositório para preencher as navegações. Simples, mas N+1 se mal usado.

Exemplo (`LotteryService.SelectLotteryById`):
```
var lottery = await _repo.SelectById(id);
lottery.Institution = await _institutionRepo.SelectById(lottery.IdInstitution);
return lottery;
```
Em listagens, **fazer um único `SelectInById(idsDistintos)`** e indexar em `Dictionary` para evitar N+1.

### B. **JOIN com Dapper `splitOn`** (como já existe em `UserRepository.SelectUserCredentialsByNickname`)
Mais performático, uma query só, mas exige escrever SQL na mão. Usar quando:
- O endpoint é hot-path.
- Há mais de uma FK a resolver e o SQL é direto.

> Recomendação: **A** como padrão; **B** apenas em rotas listagem mais usadas (`GET /institution/{idInstitution}/lottery`, `GET /lottery/{idLottery}/ticketbook`).

---

## Itens de trabalho

### 5.1 Adicionar propriedades de navegação nas Entidades

Em cada entidade, adicionar a propriedade **anotada para Dapper ignorar** (`[Dapper.Contrib.Extensions.Computed]` ou `[NotMapped]` do `DataAnnotations.Schema`) para que o `BaseRepository` (reflexão) **não** as inclua nos `INSERT`/`UPDATE`.

```
[NotMapped] public Institution? Institution { get; set; }
```

Atualizar `BaseRepository.GetProperties<T>(idColumnName)` (em `Edoha.Infraestructure/Repositories/BaseRepository.cs`) para também filtrar `[NotMapped]`:

```
return typeof(T).GetProperties()
    .Where(p => p.Name != idColumnName
             && p.GetCustomAttribute<NotMappedAttribute>() is null);
```

Esta alteração é **pré-requisito** dos próximos passos.

### 5.2 Lottery → Institution

**Entidade**: adicionar `[NotMapped] public Institution? Institution { get; set; }` em `Lottery`.

**Service** (`LotteryService`):
- `SelectLotteryById`: hidratar `Institution`.
- `SelectAllLotteries`: opção A (preferir B se a tabela ficar quente).

### 5.3 Ticketbook → Lottery + StatusTicketbook

**Entidade `Ticketbook`**: `[NotMapped] public Lottery? Lottery { get; set; }` e `[NotMapped] public StatusTicketbook? Status { get; set; }`.

**Service** (`TicketbookService.SelectAllTicketbooks` / `SelectById` / `SelectReturneds` / `SelectWithdrawns` / `GetTicketbookByNumber`):
- Hidratar `Status` (poucos status, tabela pequena → fazer **um** `SelectAll` em `StatusTicketbookRepository` e usar dicionário em memória).
- Hidratar `Lottery` somente em `SelectById` e `GetTicketbookByNumber` (em listagens, evitar para não pesar).

### 5.4 UserInstitution → User + Institution

**Entidade**: passar a herdar de `Entity` (corrige bug em paralelo — o `CreateAt`/`CreateBy` é typo de `CreatedAt`/`CreatedBy`). Adicionar navegações.

**Service** (`UserInstitutionService`): hidratar `User` e `Institution` em GETs. Útil para o frontend listar “quais usuários têm acesso a tal instituição”.

### 5.5 UserPermission → Page + Action + Permission + User

`UserPermissionService.GetUserPermissionsGroupByPageName` já retorna projeção `UserPermissionExpand` com `PageName`/`ActionName`. Esta projeção é boa — manter como `Response`. Para o **CRUD** simples (GET por id), hidratar Page/Action/Permission/User como navegações opcionais.

### 5.6 Ticketbook.Tickets — caso especial

Já é hidratado em `SelectReturneds`/`SelectWithdrawns`/`SelectById`/`GetTicketbookByNumber`. Mas faz **N+1** rodando `SelectAllByTicketbook` em loop:

```
foreach (var ticketbook in ticketbooks) {
    ticketbook.Tickets = (await _ticketRepository.SelectAllByTicketbook(ticketbook.Id)).ToList();
}
```

Trocar por **um SELECT único** `WHERE id_ticketbook = ANY(@Ids)` + `GroupBy` em memória. Adicionar método `ITicketRepository.SelectAllByTicketbookIds(IEnumerable<Guid>)`.

### 5.7 GET /institution/{idInstitution}/lottery — listagem com Institution já no path

Caso especial: o `idInstitution` está no path; **já é conhecido**. Em vez de fazer query para a institution, basta **uma** chamada e setar a mesma Institution em todos os Lotteries da resposta. Otimização barata.

### 5.8 Padrão de paginação (opcional, mas correlato)

A maioria dos GETs lista **tudo**. Quando expandimos relacionamentos, a resposta cresce. Avaliar adicionar `?page=&size=` em pelo menos:
- `GET /lottery/{idLottery}/ticketbook`
- `GET /ticketbook/{idTicketbook}/ticket`

Se for adotado, alterar o response para `PagedResponse<T> { Items, Page, Size, Total }` em `Edoha.Domain/Models/Responses/Common/`.

> Esta sub-task é **opcional** — se complicar, deixar como TODO documentado e seguir adiante.

---

## Critérios de aceitação

- [ ] `BaseRepository` ignora propriedades `[NotMapped]` em INSERT/UPDATE/SELECT (mantendo o select * funcional ainda permite preencher se a coluna não existe — Dapper silenciosamente ignora).
- [ ] `Lottery`, `Ticketbook`, `UserInstitution`, `UserPermission` têm navegações populadas em pelo menos `GetById`.
- [ ] N+1 do `Ticketbook.Tickets` substituído por consulta em lote.
- [ ] `LotteryController.GetAll(idInstitution)` retorna lotteries com `Institution` setado a partir do id do path (sem ir ao banco para isso).
- [ ] Ao serializar uma entidade que **não foi hidratada**, o JSON omite o campo (`JsonSerializerOptions.DefaultIgnoreCondition = WhenWritingNull` em `Program.cs` se ainda não tiver) ou retorna `null` claro.
- [ ] Build OK e Swagger continua refletindo as entidades expandidas.

---

## Notas para a IA executora

- **Ordem sugerida** dentro desta task: 5.1 (base) → 5.2 → 5.3 → 5.6 → 5.4/5.5 → 5.7. Cada bullet é um commit pequeno.
- Verificar serialização circular: `Lottery.Institution` ⇄ `Institution.Lotteries`? Se a `Institution` ganhar lista de `Lotteries` no futuro, vai dar loop. **Não** adicionar coleções inversas neste momento.
- Para evitar inflar payloads, considerar querystring `?expand=institution,status` que ativa a hidratação. Ficar como **TODO** (não obrigatório nesta task) para não explodir escopo.
