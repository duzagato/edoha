# 04 — Padronizar modelos: Requests, Responses e Entidades

> **Dependência:** Recomenda-se concluir **task 03** antes (controllers magros, namespaces ok). Pode ser feita em paralelo com 02.
> **Complexidade:** 🟡 Média — bastante renomeação e reorganização; pouca lógica nova.

---

## Definição (alinhada com o problema relatado pelo dono do projeto)

| Tipo | Quando usar | Pasta destino |
|---|---|---|
| **`*Request`** | Sempre que o cliente **envia** dados (POST/PUT/PATCH body, ou input estruturado). Substitui os atuais `Create*DTO`/`Update*DTO`/`*InputModel`/`Post*Request`. | `Edoha.Domain/Models/Requests/<Aggregate>/` |
| **Entidade** (`User`, `Lottery`, `Ticketbook`, `Institution`, ...) | **Retorno padrão** de GETs. Inclusive para listas. | `Edoha.Domain/Entities/` |
| **`*Response`** | Apenas quando o GET precisa de algo **diferente** da entidade (subset, projeção agregada, dados não-persistidos como `AccessToken`). Ex.: `AuthResponse`, `UserPermissionPageResponse`. | `Edoha.Domain/Models/Responses/<Aggregate>/` |

> Não existirá mais a pasta `DTOs/` nem `InputModels/`. Tudo migra.

---

## Inventário de mudanças

### 4.1 Pasta `Models/DTOs/` → migrar para `Models/Requests/`

| Arquivo atual | Destino |
|---|---|
| `DTOs/Lottery/CreateLotteryDTO.cs` | `Requests/Lottery/CreateLotteryRequest.cs` (já existe um `CreateLotteryRequest`, reconciliar — manter o de `Requests/`, apagar o duplicado) |
| `DTOs/Lottery/UpdateLotteryDTO.cs` | `Requests/Lottery/UpdateLotteryRequest.cs` |
| `DTOs/Ticket/CreateTicketDTO.cs` | `Requests/Ticket/CreateTicketRequest.cs` (já existe — reconciliar) |
| `DTOs/Ticket/UpdateTicketDTO.cs` | `Requests/Ticket/UpdateTicketRequest.cs` |
| `DTOs/UserInstitution/CreateUserInstitutionDTO.cs` | `Requests/UserInstitution/CreateUserInstitutionRequest.cs` |
| `DTOs/UserInstitution/UpdateUserInstitutionDTO.cs` | `Requests/UserInstitution/UpdateUserInstitutionRequest.cs` |
| `DTOs/Institution/CreateInstitutionDTO.cs` | `Requests/Institution/CreateInstitutionRequest.cs` |
| `DTOs/Institution/UpdateInstitutionDTO.cs` | `Requests/Institution/UpdateInstitutionRequest.cs` |
| `DTOs/Permission/CreatePermissionDTO.cs` | `Requests/Permission/CreatePermissionRequest.cs` |
| `DTOs/Permission/UpdatePermissionDTO.cs` | `Requests/Permission/UpdatePermissionRequest.cs` |
| `DTOs/StatusTicketbook/CreateStatusTicketbookDTO.cs` | idem |
| `DTOs/StatusTicketbook/UpdateStatusTicketbookDTO.cs` | idem |
| `DTOs/Page/CreatePage.cs` | `Requests/Page/CreatePageRequest.cs` |
| `DTOs/Page/UpdatePage.cs` | `Requests/Page/UpdatePageRequest.cs` |
| `DTOs/User/UpdateUserDTO.cs` | `Requests/User/UpdateUserRequest.cs` |
| `DTOs/User/CreateUserDTO.cs` | **mesclar** com `Requests/User/CreateUserRequest.cs` (vinda do `CreateUserInputModel`). O DTO atual carrega `byte[]? Password` (já hash); o input é a senha em claro. Manter **dois arquivos** semânticos: `CreateUserRequest` (público, com `UnhashedPassword`) e `CreateUserCommand` (interno, com `byte[] Password`) — ou resolver internamente sem novo tipo, ver 4.5. |
| `DTOs/UserType/...` | `Requests/UserType/...` |
| `DTOs/Action/...` | `Requests/Action/...` |
| `DTOs/Ticketbook/CreateTicketbookDTO.cs` | mesma observação que `CreateUser` — o `PostTicketbookRequest` (público) já existe; manter `Ticketbook/CreateTicketbookCommand` interno se necessário |
| `DTOs/Ticketbook/UpdateTicketbookDTO.cs` | `Requests/Ticketbook/UpdateTicketbookRequest.cs` |
| `DTOs/Ticketbook/TicketbookConfigurationDTO.cs` | `Requests/Ticketbook/TicketbookConfigurationRequest.cs` (avaliar uso — se só interno, virar `Command`) |
| `DTOs/UserPermission/CreateUserPermissionDTO.cs` | `Requests/UserPermission/CreateUserPermissionRequest.cs` |
| `DTOs/UserPermission/UserPermissionExpand.cs` | `Responses/UserPermission/UserPermissionExpand.cs` (é projeção SQL) |
| `DTOs/UserPermission/UserPermissionPage.cs` | `Responses/UserPermission/UserPermissionPageResponse.cs` |
| `DTOs/User/UserCredentials.cs` | revisar uso; provavelmente `Requests/Auth/CredentialsRequest.cs` |
| `DTOs/User/UserInformation.cs` | é input para `InsertUserInformation` — virar `Requests/User/CreateUserInformationCommand.cs` (interno) |
| `DTOs/Auth/CredentialsDTO.cs` | `Requests/Auth/CredentialsRequest.cs` |
| `DTOs/DTO.cs` (base abstrata com `GetProperties`) | **manter por hora** em `Models/Common/DTO.cs` — ainda é usada pelo `BaseRepository`. Após a task 03/04 concluírem, considerar substituí-la por reflexão direta no `BaseRepository` ou por `Requests` herdarem de uma `RequestBase` equivalente. |

### 4.2 Pasta `Models/InputModels/` → eliminar

`InputModels/User/CreateUserInputModel.cs` já é coberto pela task 03 (vira `Requests/User/CreateUserRequest.cs`).

### 4.3 Pasta `Models/Login/` → mover para `Requests/Auth/` ou `Commands/Auth/`

`InsertLoginInformationsWithoutTokenExpiration.cs` é um command interno do `AuthService`. Mover para `Edoha.Domain/Models/Commands/Auth/InsertLoginCommand.cs`. (Ou manter no Application Service se a task 03 introduziu Application layer.)

### 4.4 Pasta `Models/Responses/`

Adicionar conforme necessidade:
- `Responses/Auth/AuthResponse.cs` (mover de `Requests/AuthResponse.cs`).
- `Responses/User/UserInformationResponse.cs` (já existe).
- `Responses/UserPermission/UserPermissionPageResponse.cs` (vindo de `DTOs`).

> **Regra:** se um GET já consegue retornar a Entidade tal qual, **não criar Response**. Só criar Response quando for projeção/agregação/dados extras.

### 4.5 Mapeamento Request → Entidade dentro do Service

O Service recebe `*Request` e:
1. Valida (via `IRequestValidationContext`).
2. Cria a **entidade** (ou um command interno) preenchendo campos derivados (hash de senha, datas, FKs do path).
3. Persiste passando a entidade ou um “DTO interno” (compatível com `BaseRepository`).

Exemplo (`UserService.InsertUser`):

```
public async Task InsertUser(CreateUserRequest request) {
    await _requestValidationContext.ValidateRequest(request);
    var hashed = HashPassword(request.UnhashedPassword);
    var user = new User {
        Name = request.Name,
        Phone = request.Phone,
        Nickname = request.Nickname,
        Password = hashed
    };
    await _userRepository.Insert(user);
}
```

> A persistência via `BaseRepository` hoje aceita `DTO`. Após task 03 (item 3.14) `BaseRepository` aceitará `object`/Entity. Caso ainda dependa de `DTO`, usar um `CreateUserCommand : DTO` interno temporário.

### 4.6 GETs devolvem **Entity**

Hoje:
- `UserController.GetById` → `User` ✅
- `UserController.GetUsersWithTicketbooks` → `UserInformationResponse` (Response) — **mantém** (é projeção).
- `InstitutionController.GetAll` → `IEnumerable<Institution>` ✅
- `LotteryController` → `IEnumerable<Lottery>` ✅
- `TicketbookController.GetById` → `Ticketbook` (com `Tickets` carregados) ✅

Não há mudança aqui — é só formalizar que esse é o padrão.

### 4.7 Atualizar referências

Ferramentas para isso:
- IDE/`dotnet`: rename refactor com confiança.
- `grep -r "DTO"` em `Edoha.Application` para varrer controllers usando ainda os tipos antigos.
- Atualizar `using` namespaces.

### 4.8 Validar Swagger

Após renomear, todos os schemas no Swagger devem aparecer como `CreateXxxRequest`/`UpdateXxxRequest`/`XxxResponse`. Verificar manualmente.

---

## Critérios de aceitação

- [ ] Pasta `Models/DTOs/` não existe mais (exceto `Models/Common/DTO.cs` se ficou como base de reflexão — opcional).
- [ ] Pasta `Models/InputModels/` não existe mais.
- [ ] Toda classe enviada por **request body** está em `Models/Requests/<Aggregate>/<Action><Aggregate>Request.cs`.
- [ ] Toda classe usada como response **subset/agregada** está em `Models/Responses/<Aggregate>/`.
- [ ] `AuthResponse` está em `Responses/Auth/`.
- [ ] Controllers (`[FromBody]`) usam apenas `*Request` (não mais `*DTO`/`*InputModel`).
- [ ] GETs simples retornam entidades; só GETs especiais retornam `*Response`.
- [ ] `dotnet build src/Edoha.sln` OK.

---

## Riscos e mitigação

- **Quebra do mapeamento Dapper**: o `BaseRepository<T>` lê props de `T` (entidade), e o `Insert(DTO)` lê props do DTO via reflexão. Ao trocar DTOs por Requests, garantir que os nomes das **propriedades** continuam batendo com as **colunas** do banco (PascalCase → snake_case via `StringHelper`). Caso contrário, queries quebram silenciosamente.
- **Campos calculados**: `CreateUserDTO.Password` (`byte[]`) ≠ `CreateUserRequest.UnhashedPassword` (`string`). O Service deve manter um `CreateUserCommand` interno ou mapear para a entidade direto.
- **Entidades retornadas com FKs cruas**: este é o problema da **task 05** — não é resolvido aqui. Em GETs, o consumidor ainda verá `Ticketbook.IdLottery: Guid` puro até a task 05 expandir.
