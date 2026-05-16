# 07 — Onde colocar regras de negócio (Entidade vs VO vs Service)

> **Dependência:** **task 03** (camadas) e **task 06** (VOs prontos para receber regras finas).
> **Complexidade:** 🔴 Alta — exige análise por agregado.

---

## Resposta curta à pergunta do dono do projeto

> *“Comecei colocando tudo no service. Não sei se é a melhor decisão. Vi casos em que as regras ficam encapsuladas nas Entidades de Domínio ou até no ValueObjects. Qual o melhor plano?”*

A resposta correta é **híbrida**, e o critério é **a quem a regra pertence conceitualmente**:

| Tipo de regra | Onde colocar | Exemplo neste projeto |
|---|---|---|
| Invariante de **um valor** isolado | **Value Object** (no construtor) | Senha tem 8–30 chars; Phone tem 10 ou 11 dígitos; CPF tem dígitos verificadores válidos |
| Invariante de **um agregado** (entidade-raiz) | **Entidade** (em métodos, não em props) | Talão devolvido não pode ser retirado de novo; lottery não pode ter `NumTicketsTicketbook <= 0` |
| Regra que envolve **mais de um agregado** ou recursos externos (DB, HTTP) | **Domain Service** | Ao criar um talão, verificar unicidade de número dentro da loteria |
| Orquestração (transação, commit, mapping de Request → Entity, autorização ampla) | **Application Service** (= Service de hoje, mas “mais magro”) | `AuthService.Autenticate`, `TicketbookService.InsertTicketbook` (parte “orquestra”, parte “regra”) |
| Resposta HTTP, status, model binding | **Controller** | n/a |

---

## Plano de migração concreto, por agregado

### 7.1 `User`

#### Hoje
- `UserService.HashPassword` valida tamanho da senha **+** chama crypto.
- `UserService.IsUsernameSended/IsPasswordSended` são checagens de string vazia.
- `UserService.ValidateUserCredentials` busca usuário e compara hash.

#### Para onde mover
- Validar senha → **`Password` (VO)** (task 06).
- Validar nickname → **`Nickname` (VO)** ou na **entidade** se for invariante de `User`.
- `User.ChangePassword(Password newPassword, ICrypto crypto)` → **método na entidade** (regra: senha sempre vira hash; nunca expor `byte[]` direto).
- `User.MatchesPassword(string raw, ICrypto crypto)` → **método na entidade**, encapsula a comparação. O service só faz `if (!user.MatchesPassword(...)) throw ...`.
- `UserService` continua orquestrando: buscar do repo, montar `RequestValidationException` para o pipeline HTTP.

### 7.2 `Lottery`

#### Hoje
- `LotteryService.InsertLottery`: verifica unicidade do nome e seta `IdInstitution` no DTO.
- Validações estão como `[Required]` na entidade e DTO.

#### Para onde mover
- **Construtor da entidade** (`Lottery.Create(idInstitution, name, numTicketsTicketbook, numTicketbooks, priceTicket, doubleChance)`) com validações:
  - `numTicketsTicketbook > 0`, `numTicketbooks > 0`, `priceTicket > 0`.
  - `name` não vazio.
- **Service** (Domain): unicidade do nome **dentro da instituição** (envolve repository → permanece no service).
- **Application Service**: orquestrar `idInstitution` vindo do path + Request.

### 7.3 `Ticketbook`

#### Hoje
- `TicketbookService.InsertTicketbook`: verifica unicidade do número, cria owner via `_userService.InsertUserInformation`, cria holder, monta DTO, ajusta `DevolutionDate` se status == `Devolvido`.
- `ChangeTicketbookStatus`/`ChangeTicketbookStatusToReturned`/`ChangeTicketbookStatusToWithdraw` chamam o repositório direto.

#### Para onde mover
- **Entidade `Ticketbook`** ganha métodos:
  - `MarkAsReturned()`: muda `IdStatusTicketbook` para `Devolvido`, define `DevolutionDate = DateTime.UtcNow`. Lança `InvalidOperationException`/`DomainException` se já estava devolvido.
  - `MarkAsWithdrawn()`: muda para `Retirado`, define `WithdrawnDate = UtcNow`. Lança se já retirado.
  - `ChangeStatus(int idStatus)`: regra mais permissiva, permitida em casos de correção administrativa.
- O service (`TicketbookService.ChangeTicketbookStatusToReturned`) vira:
  ```
  var tb = await _repo.SelectById(idTicketbook);
  tb.MarkAsReturned();
  await _repo.Update(tb);
  ```
- **Service** (Domain) mantém:
  - Verificação de unicidade do `Number` dentro da `Lottery` (acessa repositório).
  - Orquestração de criação do `Owner` (que é uma `UserInformation` — ver discussão abaixo).

#### Decisão pendente: Owner/Holder
Hoje `TicketbookOwner`/`TicketbookHolder` são entidades separadas armazenadas em `User` (via `_userService.InsertUserInformation`). Isso mistura conceitos:
- “Pessoa que retirou o talão” não é necessariamente um usuário do sistema.
- Modelar como **VO `Person { Name, Phone, Cpf? }`** dentro do agregado `Ticketbook` faz mais sentido. O `IdOwner`/`IdHolder` viram **detalhe de persistência** (FK para uma tabela `person`), não um conceito de domínio.

> **Recomendação:** introduzir `Person` (VO ou Entity-of-aggregate) e mover esse fluxo para fora de `UserService`. Se for muito invasivo, deixar **TODO** documentado.

### 7.4 `Institution`

- **Entidade**: validações de nome/short name/slug movidas para construtor, usando `Slug` (VO da task 06).
- **Service**: continua para `SelectInstitutionsByUser` (envolve outro repo).

### 7.5 `UserPermission`

- **Service** continua sendo o centro — a regra é **agregação cruzada** (User × Page × Action). Não há entidade rica que faça sentido aqui.
- Manter `Service` orquestrando, com regra explicita: **sem permissão para um par (page, action) ⇒ 403**. Já está parcialmente em `PermissionHandler`. **Mover** a checagem para um método de domínio `UserPermissionPolicy.CanAccess(idUser, page, action)` chamável tanto pelo handler quanto por services que precisem checar fora de HTTP.

### 7.6 `Login` / Auth

- `Login.MarkRevoked(DateTime when)` — método da entidade.
- `Login.IsActive(DateTime now)` — método da entidade (`!Revoked && (ExpiresAt is null || ExpiresAt > now)`).
- `AuthService` orquestra; não decide por si só se o login está válido.

### 7.7 `Ticket`

- Pequeno; verificar se há regra de “sorteado” / “premiado”. Se sim, virar método na entidade.
- Caso contrário, continuar como CRUD simples.

---

## Anti-padrão a remover

- **Anemia explícita**: setters públicos em propriedades de domínio + zero comportamento. Após esta task, o setter público vira raro; criação via construtor/factory.
- **`[Required]` como única regra**: substituído por checagem no construtor/método. (`[Required]` em **Request** continua válido — é regra de transporte/validação de input, não de domínio.)
- **Validações duplicadas** (Request + Entity + Service): manter em **um lugar**, preferindo o **mais baixo** (VO > Entity > Service).

---

## Plano sugerido de execução

Ordem (cada item é um commit ou um par de commits):

1. **Estabelecer convenção** num doc curto em `backlog/` ou no README de `Edoha.Domain` — usar este arquivo como base.
2. Refatorar `User` (consumidor: `UserService`, `AuthService`, `UserController`).
3. Refatorar `Lottery`.
4. Refatorar `Ticketbook` (status changes → métodos na entidade).
5. Refatorar `Institution`.
6. Adicionar `UserPermissionPolicy` e mudar `PermissionHandler` para consumi-la.
7. Refatorar `Login` (`MarkRevoked`/`IsActive`).
8. Avaliar `Person` para `Owner`/`Holder`.

---

## Critérios de aceitação

- [ ] Entidades não têm mais validações no Service que poderiam estar em VO/método da entidade.
- [ ] `Ticketbook.MarkAsReturned()` / `MarkAsWithdrawn()` existem; `TicketbookService` os utiliza.
- [ ] `User.ChangePassword`/`MatchesPassword` existem; `UserService` mais magro.
- [ ] Não existem mais setters públicos em `Lottery`/`Institution`/`User` para campos com invariantes (substituídos por métodos).
- [ ] `dotnet build` passa; Swagger não regride.
- [ ] Documento curto em `backlog/` (ou `docs/`) registrando a convenção “regra na VO/entidade/service” usada como referência futura.

---

## Notas para a IA executora

- Em cada iteração, validar manualmente o endpoint correspondente via Swagger.
- Cuidado com **construtor sem parâmetros** exigido pelo Dapper. Solução: manter `protected` constructor sem parâmetros + `public static Create(...)` factory + `public init`/`public set` apenas no que o ORM precisa. Alternativa: adicionar **`Dapper.SqlMapper.SetTypeMap`** para mapear via campos privados (mais trabalho).
- Manter `[Required]` nos **Requests** (input de transporte) — não confundir com domínio.
