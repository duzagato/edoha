# Edoha — Guia para Claude

## O que é
API REST em .NET 8 (ASP.NET Core) para gestão de rifas/loterias. Multi-tenant por instituição. Banco PostgreSQL via Dapper (sem EF Core).

## Projetos (src/)
| Projeto | Responsabilidade |
|---|---|
| `Edoha.Application` | Entry point: Controllers, Middlewares, DI wiring, Program.cs |
| `Edoha.Domain` | Regras de negócio: Entities, Services, Interfaces, DTOs, Models |
| `Edoha.Infraestructure` | Infra: Repositories, JWT, Handlers, Utils (Crypto, Json) |
| `Edoha.Shared` | Anotações de validação, Helpers, Exceptions compartilhadas |

## Schemas do banco
- `edoha` → usuarios, instituições, permissões (user, institution, user_institution, permission, user_permission, page, action, permission_page_action, user_type, login, table_configuration)
- `lottery` → rifas (lottery, ticketbook, ticket, status_ticketbook)

## Entidades principais
- **Institution** — organização que cria rifas
- **Lottery** — rifa de uma instituição (numTicketbooks, numTicketsTicketbook, doubleChance)
- **Ticketbook** — talão (Owner + Holder; status: 1=Retirado, 2=Devolvido)
- **Ticket** — bilhete individual de um talão
- **User** — usuário; autenticado por nickname+password (bcrypt)
- **Permission/UserPermission** — autorização por Page+Action (controller+método HTTP)

## Padrões de código
- **Repository**: `BaseRepository<T>` gera SQL dinamicamente via reflection + `[Table]` + snake_case. Queries complexas ficam em `StaticQueries.cs`.
- **Service**: `Service<T>` base com Insert/Update/Delete genéricos. Cada service concreto implementa regras específicas.
- **Validação**: `IRequestValidationContext.ValidateDTO(dto)` → lança `RequestValidationException` → capturado pelo `ExceptionHandlingMiddleware` → HTTP 400 com `{is_valid, errors}`.
- **DI**: Scoped. Registrado em `ServiceInjection`, `RepositoryInjection`, `UtilsInjection`, `JwtInjection`.
- **Conversão de nomes**: PascalCase → snake_case via `StringHelper.PascalToSnakeCase`.
- **DTO**: Herdam de `DTO` (base). `GetProperties()` exclui `Id` na geração de INSERT/UPDATE.

## Autenticação / Autorização
- JWT (JwtBearer). Claims: `NameIdentifier=userId`, `Name=nickname`.
- Expiração: 60 min (configurável em `appsettings.json > Jwt:ExpiresInMinutes`).
- `PermissionHandler` valida por controller (page) + método HTTP (action) via `vw_user_permission_page`.
- Policy: `"PermissionPolicy"` — aplicar com `[Authorize(Policy = "PermissionPolicy")]`.

## Middleware
- `ExceptionHandlingMiddleware`: converte `RequestValidationException` → 400, `SecurityTokenException` → 401, genérico → 500.

## CI/CD (GitHub Actions)
- `feature/*` / `fix/*` → PR automático para `develop`
- `develop` → `release` → `main` (workflows sequenciais)

## Convenções
- Banco: snake_case. C#: PascalCase.
- Erros de validação: via `_requestValidationContext.AddError(key, message)` antes de chamar `ValidateDTO`.
- Não há testes automatizados no projeto atualmente.

---

## Estrutura docs/

### docs/backlog/AI/
Cada arquivo `.md` é uma task para o Claude executar. Formato:
```
Dependência: [nenhuma | task-XX]
Dificuldade: [baixa | média | alta]
---
Descrição da tarefa...
```

### docs/backlog/Human/
Criado após cada task do AI ser concluída. Explica intervenções humanas necessárias para finalizar a task (ex: rodar migration, configurar variável de ambiente, fazer deploy).

### docs/changes/
Um arquivo `.md` por alteração realizada. Explica o que foi feito, quais arquivos foram modificados e por quê, para que o usuário possa revisar e avaliar.
