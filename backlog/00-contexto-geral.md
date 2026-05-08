# 00 — Contexto Geral do Projeto

> Documento-base. Toda task subsequente parte do contexto descrito aqui.
> **Não tem dependência.** Leitura obrigatória antes de qualquer outra task.

---

## 1. Visão geral

O **Edoha** é uma **API REST** escrita em **C# / .NET 8 (ASP.NET Core)** que serve um frontend (Angular, baseado em `policy.WithOrigins("http://localhost:4200")`). O domínio principal é gestão de **rifas/sorteios** (lottery), com **talões de tickets** (ticketbook), **tickets**, **instituições** que rodam essas rifas, **usuários** com **permissões granuladas por página/ação** e um sistema de **autenticação por JWT**.

Banco de dados: **PostgreSQL** (Npgsql + Dapper). Há um arquivo `edoha.sql` na raiz do repositório (atualmente vazio/placeholder).

Logging: **Serilog** com enriquecimento de CorrelationId.

Autenticação: **JWT Bearer** já registrado em `JwtInjection`, mas sem `[Authorize]` aplicado nos controllers (autenticação não está realmente sendo exigida — ver task 02).

## 2. Estrutura de pastas (solução `src/Edoha.sln`)

```
src/
├── Edoha.Application/        ← Web API (Controllers, Program.cs, DI, Middlewares)
├── Edoha.Domain/             ← Entidades, Models (DTOs/Requests/Responses), Interfaces, Services (regra de negócio), Constants, Annotations, Exceptions
├── Edoha.Infraestructure/    ← Repositórios (Dapper), Handlers (PermissionHandler), Services de infra (TokenGenerationService), Util (Crypto, Json, SystemUtils), Context (RequestValidationContext, RequestContext, DbConnectionContext), Constants (JwtConfig, StaticQueries)
└── Edoha.Shared/             ← Annotations, Exceptions, Helpers compartilhados
```

> Observação: o nome da pasta é `Edoha.Infraestructure` (com "e" extra) — typo histórico mantido em namespaces; tasks devem **preservar** esse nome para não quebrar referências.

### Camadas e responsabilidades atuais

| Camada | Pasta | Estado |
|---|---|---|
| Apresentação (HTTP) | `Edoha.Application/Controllers` | Controllers verbosos, com `try/catch` repetido, herdando `ControllerBase` direto |
| Aplicação (orquestração) | — | Não existe camada de Application Services separada; lógica está misturada em Domain Services |
| Domínio | `Edoha.Domain/Entities`, `Edoha.Domain/Services` | Entidades anêmicas (apenas getters/setters); regras estão nos Services |
| Infra | `Edoha.Infraestructure/Repositories` | Dapper + reflexão para mapear pascal/snake_case via `BaseRepository<T>` |

## 3. Stack e dependências

- **.NET 8** (`<TargetFramework>net8.0</TargetFramework>`)
- **Microsoft.AspNetCore.Authentication.JwtBearer 8.0.18**
- **Dapper** (via `BaseRepository`)
- **Npgsql** (PostgreSQL)
- **Serilog 4.x** + Serilog.AspNetCore + Enrichers.CorrelationId
- **Swashbuckle.AspNetCore 6.6.2** (Swagger)
- **Microsoft.Data.SqlClient 6.0.1** (presente, mas o projeto usa Postgres — provavelmente legado e pode ser removido)

## 4. Componentes-chave já existentes

- `Program.cs` — bootstrap mínimo, `UseAuthentication`/`UseAuthorization` registrados.
- `JwtInjection.AddJwt` — configura JwtBearer + handler de eventos 401/403 + `PermissionHandler` registrado para a policy `"PermissionPolicy"`.
- `ExceptionHandlingMiddleware` — captura `RequestValidationException`, `SecurityTokenException`, `UnauthorizedAccessException` e genérica.
- `RequestValidationContext` — coleta erros de validação por request e roda `Validator.TryValidateObject` em DTOs.
- `BaseRepository<T>` — CRUD genérico via reflexão e Dapper, monta SQL automaticamente a partir do nome da classe e atributo `[Table]`.
- `PermissionHandler` — autorização baseada em controller/método HTTP; usa claim `NameIdentifier` para extrair `idUser`.
- `TokenGenerationService` — gera JWT e refresh token (random 32 bytes em base64).
- `AuthService.Autenticate` — valida credenciais, gera token, persiste login e devolve `AuthResponse { IdUser, AccessToken, Institutions }`.

## 5. Convenções e padrões adotados (alguns inconsistentes)

- DI organizada em arquivos: `RepositoryInjection`, `ServiceInjection`, `JwtInjection`, `UtilsInjection` chamados por extensions em `ServiceCollection.cs` (typo: `ServoceCollection`).
- Models divididos em três pastas em `Edoha.Domain/Models`:
  - `DTOs/` — usado para Create/Update e até para Auth (`CredentialsDTO`).
  - `InputModels/` — só `User/CreateUserInputModel` (inconsistente).
  - `Requests/` — `CreateLotteryRequest`, `CreateTicketRequest`, `Ticketbook/PostTicketbookRequest`, e `AuthResponse` (response **dentro** da pasta Requests — bug organizacional).
  - `Responses/User/UserInformationResponse` — começou a aparecer só em um lugar.
- Entidades herdam de `Entity` (Id, CreatedAt, CreatedBy) — exceto `Login`, `UserPermission`, `UserInstitution` (não herdam).
- Annotations customizadas: `Min`, `Max`, `MinLength`, `MaxLength`, `RangeLength` (várias estão **vazias** — `MaxLength.cs`, `RangeLength.cs` são classes internas vazias).
- Repositórios usam `Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true` (setado várias vezes, deveria ser global em `Program.cs`).

## 6. Banco de dados

- Schemas usados: `edoha` (default), `lottery` (Ticketbook, Lottery).
- Conexão: `IDbConnection` registrado como **Singleton** em `ServoceCollection.AddDatabase` — **anti-padrão** (Npgsql não é thread-safe assim; deveria ser `Scoped` ou usar `IDbConnectionFactory`/`NpgsqlDataSource`).
- Senha em texto-claro em `appsettings.json`: `Password=12341234` — secret leak.
- `IDbConnectionFactory` está declarada em `Edoha.Domain/Interfaces/Infraestructure/Context` mas a implementação em `Edoha.Infraestructure/Context/DbConnectionContext.cs` parece estar definida e não usada — verificar e padronizar.

## 7. Endpoints (prefixos)

```
POST   /auth                                 — login
GET    /user, /user/{id}, /user/user_information
POST   /user, PUT /user, DELETE /user/{id}
GET    /institution, /institution/{slug}, /institution/institution_by_user/{idUser}
POST   /institution, PUT /institution, DELETE /institution/{id}
GET    /institution/{idInstitution}/lottery[/...]
GET    /lottery/{idLottery}/ticketbook[/...]
GET    /ticketbook/{idTicketbook}/ticket[/...]
GET    /userpermission/{id}, POST /userpermission, DELETE /userpermission/{id}
... (page, action, permission, statusticketbook, usertype, userinstitution, tableconfiguration)
```

Nenhum endpoint exige autenticação atualmente (não há `[Authorize]` em nenhum controller — confirmado por inspeção).

## 8. Workflows / CI

`.github/workflows/`:
- `1. pr-to-develop.yml` — abre PR para develop em push em `feature/*` / `fix/*`.
- `2. merge-develop.yml` — cria branch `release/x.y.z` e PR develop → release ao mergear PR em develop.
- `3. merge-release.yml` — após merge em release/*, mergeia develop nela e abre PR para main. **Contém marcadores de conflito Git não resolvidos (`<<<<<<< HEAD ... >>>>>>>`)** — bug crítico.
- `4. merge-main.yml` — checa conflitos pós-merge em main.

Não existe workflow de **build/test/CodeQL/lint** nem de **deploy** (CI de qualidade e CD para AWS são ausentes).

## 9. Dores e débitos técnicos sumarizados

Esta lista é apenas índice; cada item tem detalhamento próprio em sua task `.md`.

1. **Sem deploy AWS configurado** (sem Dockerfile, sem CI/CD para ECR/ECS, segredos hardcoded, conexão `Singleton`, configurações ambiente-locked) → **task 01**.
2. **JWT existe mas não autentica** — nenhum `[Authorize]` em controllers; `JwtConfig` tem chave hardcoded em código além de `appsettings.json` (duplicada); refresh token gerado mas nunca usado → **task 02**.
3. **Camadas embaralhadas** — Application Services inexistente; regra em Service de domínio; controllers tomam decisões HTTP+orquestração; `IRequestValidationContext` é Domain mas vive em Infra → **task 03**.
4. **Models desorganizados** — `DTO`s misturados com `InputModels`/`Requests`/`Responses`; `AuthResponse` dentro de `Requests/`; GETs retornam ora `Entity`, ora DTO (`UserInformationResponse`) → **task 04**.
5. **FKs cruas em respostas** — `Ticketbook.IdOwner/IdHolder`, `Lottery.IdInstitution`, `UserInstitution.IdUser/IdInstitution`, `UserPermission.IdPage/IdAction/...` retornam só Guids → **task 05**.
6. **Sem Value Objects** — `Phone`, `Cpf` (não existe ainda no modelo), `Email`, `Password`, `Money` (PriceTicket é `decimal` direto), `Slug` são tratados como `string`/`decimal` sem invariantes → **task 06**.
7. **Entidades anêmicas** — `User`, `Ticketbook`, `Lottery` etc. não têm comportamento; tudo está em `Service` (ex.: `UserService.HashPassword`, `TicketbookService.InsertTicketbook` faz tudo) → **task 07**.
8. **Diversos débitos pequenos** — `try/catch` copiado em todos os controllers retornando StackTrace ao cliente, `RNGCryptoServiceProvider` obsoleto, `IDbConnection Singleton`, conflict markers em workflow, classes de annotation vazias, namespace `Edoha.Controllers` (sem `Application`), typo `ServoceCollection`, exceção semanticamente errada (`CannotUnloadAppDomainException`), `Dictionary` de erros não thread-safe, `IsUnique` com bug (parâmetro nomeado errado), `InsertOrGetId` com `conflictColumns` sobrescrito por hardcode `"phone"`, `Nullable` desabilitado em vários `.csproj`, `appsettings.Development.json` no `.gitignore` e ainda commitado, etc. → **task 08**.

## 10. Como construir e rodar

- Restore + build: `dotnet build src/Edoha.sln`
- Run: `dotnet run --project src/Edoha.Application/Edoha.Application.csproj`
- Swagger em dev: `https://localhost:<port>/swagger`

---

## Próximas tasks (independentes salvo dependências marcadas)

| # | Arquivo | Complexidade | Depende de |
|---|---|---|---|
| 01 | `01-preparar-deploy-aws-ecs-fargate.md` | Média | — |
| 02 | `02-refatorar-autenticacao-jwt.md` | Alta | — |
| 03 | `03-organizacao-solid-ddd.md` | Alta | — |
| 04 | `04-padronizar-modelos-request-response-entity.md` | Média | 03 (parcialmente) |
| 05 | `05-objetos-expandidos-relacionamentos.md` | Média | 04 |
| 06 | `06-value-objects.md` | Média | 03 |
| 07 | `07-regras-de-negocio-em-entidades-e-vos.md` | Alta | 03, 06 |
| 08 | `08-debitos-tecnicos-adicionais.md` | Baixa-Média | — (algumas subtasks dependem) |
