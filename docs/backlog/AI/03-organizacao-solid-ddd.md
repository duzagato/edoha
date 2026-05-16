# 03 — Organização do código (SOLID + DDD)

> **Dependência:** Nenhuma para começar. Mas **04, 05, 06, 07 dependem desta**.
> **Complexidade:** 🔴 Alta — refatoração estrutural; requer cuidado para não quebrar comportamento.
> **Estratégia:** dividir em sub-tarefas pequenas e independentes (cada subseção abaixo pode virar um commit separado).

---

## Diagnóstico

| Pecado | Onde | Por quê é problema |
|---|---|---|
| Controllers com `try/catch` repetido retornando `StackTrace` ao cliente | Todos os controllers | Vaza informação sensível, polui código, e já existe `ExceptionHandlingMiddleware` |
| Controllers herdam de `ControllerBase` direto e duplicam lógica | Todos | Sem `BaseController` com `[Authorize]` e padrões |
| `Service<T>` com `Insert(DTO)` força que toda subclasse aceite `DTO` (ainda que internamente use `Request`) | `Service.cs` | Acoplamento entre domínio e modelos de transporte |
| Camada de Application Services não existe | — | Domain Services hoje misturam orquestração HTTP-aware (ex.: `RequestValidationContext`) e regras |
| `IRequestValidationContext` é interface de Domain mas vive em `Edoha.Infraestructure/Context` | `RequestValidationContext.cs` | Acoplamento contra a regra de dependência de DDD |
| Dependências circulares entre serviços (`UserService` ↔ `UserPermissionService` via `AuthService`) | `AuthService.cs` | Acoplamento indireto |
| `UserPermissionService` recebe `IUserPermissionRepository` **duas vezes** | `UserPermissionService.cs:23` | Bug de DI — viola DRY |
| Namespace `Edoha.Controllers` (em vez de `Edoha.Application.Controllers`) | Todos os controllers exceto `AuthController`, `UserPermissionController` | Inconsistência |
| Typo `ServoceCollection` | `src/Edoha.Application/ServiceCollection.cs` | Lê feio em IDE |
| `IDbConnection` registrado como **Singleton** | `ServoceCollection.AddDatabase` | Quebra concorrência (Npgsql) |
| Annotations vazias | `Edoha.Domain/Annotations/String/MaxLength.cs`, `RangeLength.cs` | Código morto |

---

## Objetivos

1. Separar responsabilidades em camadas explícitas, respeitando DDD “clássico” simplificado:
   - **Application** (Web): controllers magros, mappings, DI bootstrap.
   - **Domain**: entidades ricas, value objects, services de domínio (puros), interfaces.
   - **Infrastructure**: persistência (Dapper), serviços externos (token, crypto), validação contextual baseada em HTTP.
2. Aderir a SOLID nos pontos críticos:
   - **SRP**: controller só faz HTTP; service só faz regra; repositório só faz I/O.
   - **DIP**: Domain define interfaces; Infrastructure implementa.
   - **OCP**: `BaseController`/`Service<T>` aberto para extensão sem reescrever.
3. Eliminar duplicidade óbvia (try/catch nos controllers).

> Esta task **não** mexe em modelos de Request/Response (task 04), nem em Value Objects (task 06), nem em regras dentro da entidade (task 07). Foca em **arrumação**.

---

## Itens de trabalho

### 3.1 Criar `BaseController`

Caminho: `src/Edoha.Application/Controllers/BaseController.cs`

```
[ApiController]
[Authorize]      // task 02
[Route("[controller]")] // ou manter rotas explícitas; ver nota
public abstract class BaseController : ControllerBase {
    protected IActionResult OkOrNoContent<T>(IEnumerable<T> items)
        => items.Any() ? Ok(items) : NoContent();
    protected IActionResult OkOrNoContent<T>(T? item) where T : class
        => item is null ? NoContent() : Ok(item);
}
```

Nota: a maioria dos controllers usa rotas customizadas (`[Route("lottery/{idLottery}/ticketbook")]`); manter as rotas como estão e só substituir herança. **Não** colocar `[Route("[controller]")]` na classe base para não conflitar.

### 3.2 Remover `try/catch` em todos os controllers

O `ExceptionHandlingMiddleware` já trata:
- `RequestValidationException` → 400 com `errors`.
- `SecurityTokenException` → 401.
- `UnauthorizedAccessException` → 401.
- `Exception` → 500 com mensagem genérica.

Adicionar à middleware:
- `KeyNotFoundException` → 404 (entidades inexistentes).
- `ArgumentException` → 400 (`SelectById` com Guid vazio etc.).

Depois, **remover** `try/catch` dos controllers, simplificando para:

```
[HttpGet]
public async Task<IActionResult> GetAll() {
    var items = await _service.SelectAllInstitutions();
    return OkOrNoContent(items);
}
```

Aplicar em: `InstitutionController`, `LotteryController`, `TicketbookController`, `TicketController`, `UserController`, `UserPermissionController`, `UserInstitutionController`, `PermissionController`, `PageController`, `ActionController`, `UserTypeController`, `StatusTicketbookController`, `TableConfigurationController`.

> Garante: nenhum controller mais retorna `StackTrace`/`InnerException` ao cliente. **Critério de aceitação obrigatório**.

### 3.3 Mover `IRequestValidationContext` para `Edoha.Domain/Interfaces/Domain/Validation/`

Hoje está em `Interfaces/Infraestructure/Context/`. Conceitualmente é um contrato de **domínio** (regra: validar inputs). A **implementação** continua em `Edoha.Infraestructure.Context.RequestValidationContext`.

**Cuidado:** vários `using` referenciam o namespace antigo (`Edoha.Domain.Interfaces.Infraestructure.Context`). Atualizar todos.

### 3.4 Corrigir typo `ServoceCollection`

Renomear classe e arquivo: `ServoceCollection` → `ServiceCollectionExtensions` em `src/Edoha.Application/ServiceCollection.cs`.

> O nome `ServiceCollection` em si pode colidir com `Microsoft.Extensions.DependencyInjection.ServiceCollection`. Preferir `ServiceCollectionExtensions`.

### 3.5 Padronizar namespace dos controllers

Trocar `namespace Edoha.Controllers` por `namespace Edoha.Application.Controllers` em todos os controllers que ainda estão no namespace antigo (`Lottery`, `Ticketbook`, `Ticket`, `Institution`, `User`, `Page`, `Action`, `Permission`, `StatusTicketbook`, `TableConfiguration`, `UserInstitution`, `UserType`).

Atualizar `using` quando necessário (raramente quebra, controllers são consumidos por reflexão).

### 3.6 Corrigir DI duplicada em `UserPermissionService`

Construtor atualmente recebe `IUserPermissionRepository repository` **e** `IUserPermissionRepository userPermissionRepository`. Manter um só (`_userPermissionRepository`) e passar para `base(...)`.

### 3.7 Mudar registro de `IDbConnection` de `Singleton` para `Scoped`

`ServoceCollection.AddDatabase`:
```
services.AddScoped<IDbConnection>(sp =>
    new NpgsqlConnection(configuration.GetConnectionString("Default")));
```

> Idealmente: registrar `NpgsqlDataSource` como Singleton e `IDbConnection` como Scoped via `dataSource.OpenConnectionAsync()`. Se o tempo for curto, **Scoped + new NpgsqlConnection** já resolve a corrupção. Documentar como TODO.

### 3.8 Reorganizar `Edoha.Domain/Models`

Estado atual:
```
Models/
├── DTOs/...               ← input + “qualquer coisa”
├── InputModels/User/...   ← só um arquivo
├── Requests/.../...
├── Requests/AuthResponse  ← Response dentro de Requests (bug)
├── Login/...              ← interno do AuthService
└── Responses/User/...
```

Após task 04 esta árvore vai mudar drasticamente. Aqui (task 03) só:
- Mover `Requests/AuthResponse.cs` para `Responses/Auth/AuthResponse.cs` (criar pasta).
- Mover `InputModels/User/CreateUserInputModel.cs` para `Requests/User/CreateUserRequest.cs` (renomear classe). Atualizar `UserService.InsertUser` e `UserController.Create`.

### 3.9 Remover annotations vazias

Apagar:
- `src/Edoha.Domain/Annotations/String/MaxLength.cs` (vazio)
- `src/Edoha.Domain/Annotations/String/RangeLength.cs` (vazio)

Conferir `Edoha.Shared/Annotations` por classes vazias e remover.

### 3.10 Remover middleware `RequestContextMiddleware` que não faz nada

`src/Edoha.Application/Middlewares/RequestContextMiddleware.cs` declara `_next` e nem implementa `InvokeAsync`. Não está registrado em `Program.cs`. Excluir o arquivo.

> Se a intenção era criar contexto por request, revisitar como parte da task 02 (claims) ou task 08 (audit log).

### 3.11 Padronizar respostas de erro do controller (delegar ao middleware)

Em todos os controllers, substituir `BadRequest("Dados incompletos ou não enviados")` (quando `request == null`) por **retorno simples** confiando que o ASP.NET fará o model binding direito. O check `if (request != null)` é redundante quando o body é obrigatório (e a validação acontece pelo `RequestValidationContext.ValidateDTO`).

Manter em apenas um lugar — quando o body for **opcional** ou faltarem ids no path, manter `BadRequest`.

### 3.12 Logger via `_logger` (Serilog) em vez de `_ilogger`

Renomear o campo nos controllers/services que usam `_ilogger` (ex.: `AuthController`, `TicketbookController`) para `_logger`. Apenas estética/consistência com o resto do código.

### 3.13 `UserPermissionController` recebe `ILogger` sem genérico

```
public UserPermissionController(ILogger ilogger, ...)
```
Trocar para `ILogger<UserPermissionController>`. **Bug** — DI vai falhar em runtime.

### 3.14 `Service<T>` aceitar Request OR DTO

Hoje `Service<T>.Insert(DTO dto)` força a hierarquia `DTO`. Quando a task 04 introduzir `*Request`, esses tipos não vão herdar de `DTO`.

Recomendação: tornar `Service<T>` mais genérico:

```
public async Task Insert<TRequest>(TRequest request) where TRequest : class
{
    await _requestValidationContext.ValidateRequest(request);
    await _repository.Insert(request);
}
```
e introduzir `IRequestValidationContext.ValidateRequest(object)` (mesma implementação do `ValidateDTO` mas sobre `object`). Manter `ValidateDTO` como *deprecated wrapper* até a task 04 concluir a migração.

> Se preferir, deixar **só anotado** nesta task e implementar a generalização junto à task 04. Ambas são aceitáveis.

---

## Critérios de aceitação

- [ ] Nenhum controller tem `try/catch` ao redor da chamada de service (exceção: casos justificados e documentados, ex.: I/O específico de upload).
- [ ] `ExceptionHandlingMiddleware` cobre `KeyNotFoundException` (404) e `ArgumentException` (400).
- [ ] `BaseController` criado e usado por todos os controllers do domínio (não pelo `AuthController` se preferir manter explícito).
- [ ] Todos controllers no namespace `Edoha.Application.Controllers`.
- [ ] `UserPermissionService` com construtor sem duplicidade.
- [ ] `IDbConnection` registrado como `Scoped`.
- [ ] `IRequestValidationContext` movido para `Edoha.Domain/Interfaces/Domain/Validation/`.
- [ ] `ServoceCollection` renomeado.
- [ ] Annotations vazias removidas.
- [ ] `RequestContextMiddleware` vazio removido.
- [ ] `UserPermissionController` recebe `ILogger<UserPermissionController>`.
- [ ] `dotnet build src/Edoha.sln` passa sem warnings novos significativos.

---

## Notas para a IA executora

- Mexer em vários controllers de uma vez é arriscado. Sugerimos commits por “grupo”:
  1. middleware + `BaseController`
  2. controllers de auth/user/institution
  3. controllers de lottery/ticketbook/ticket
  4. demais controllers
  5. ajustes em DI/namespaces
- Sempre rodar `dotnet build src/Edoha.sln` entre commits.
- **Não** alterar o contrato HTTP (rotas, verbos, payloads). Esta task é puramente estrutural.
