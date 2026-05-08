# 02 — Refatorar autenticação JWT (proteger endpoints de fato)

> **Dependência:** Nenhuma. Pode ser feita em paralelo com a task 01.
> **Recomendação:** se 01 já mudou `JwtConfig` para `IOptions<JwtSettings>`, apenas reutilizar.
> **Complexidade:** 🔴 Alta (toca em todos os controllers, no fluxo de login e no PermissionHandler).

---

## Diagnóstico do estado atual

| Item | Estado | Evidência |
|---|---|---|
| Pacote `JwtBearer` instalado | ✅ | `Edoha.Application.csproj` |
| Configuração `AddJwtBearer` | ✅ | `JwtInjection.cs` |
| `app.UseAuthentication()` / `UseAuthorization()` | ✅ | `Program.cs` |
| Endpoint de login | ✅ | `AuthController.Autenticate` |
| Geração de token + refresh | ✅ | `TokenGenerationService` |
| Persistência de login | ✅ | `LoginRepository` (apenas insert) |
| **Aplicação de `[Authorize]` nos controllers** | ❌ | Nenhum controller tem o atributo |
| **`[AllowAnonymous]` no login** | ❌ | Não tem (vai virar problema quando `[Authorize]` for global) |
| **Refresh token funcional** | ❌ | É gerado e gravado, mas nunca consumido (não há endpoint `/auth/refresh`) |
| **Revogação no logout** | ❌ | Coluna `revoked` existe na entidade `Login`, mas não há fluxo |
| **`JwtConfig` estático com chave hardcoded** | ❌ | `JwtConfig.cs` tem chave hardcoded, duplicada em `appsettings.json` |
| **`PermissionHandler` é aplicado por policy** | ⚠️ | A policy `"PermissionPolicy"` está registrada mas **nenhum controller a usa** |
| **`Login.RefreshTokenHash` armazena valor cru** | ❌ | É salvo o token base64 puro, sem hash — contradiz o nome da coluna |
| **Tipo da chave JWT vem com aspas dentro do valor** | ⚠️ | `"Key": "\"S3nh4...\""` — as aspas internas viram parte da chave |

---

## Objetivo

1. Tornar **todos os endpoints autenticados por padrão**, com exceções explícitas.
2. Centralizar a configuração JWT em `IOptions<JwtSettings>` lendo de `IConfiguration`.
3. Implementar fluxo completo: **login → access token + refresh token → refresh → logout (revoke)**.
4. Aplicar a policy de permissão (`PermissionHandler`) onde fizer sentido.
5. Hardenizar: hash do refresh token, expiração de refresh, rotation, expiração no banco coerente, claims de audiência/instituição.

---

## Itens de trabalho

### 2.1 Criar `JwtSettings` e remover `JwtConfig` estático

Arquivo novo: `src/Edoha.Infraestructure/Constants/JwtSettings.cs`

```
public class JwtSettings {
    public string Key { get; set; } = "";
    public string Issuer { get; set; } = "";
    public string Audience { get; set; } = "";
    public int ExpiresInMinutes { get; set; } = 60;
    public int RefreshExpiresInDays { get; set; } = 7;
}
```

Em `JwtInjection.AddJwt`:
- `services.Configure<JwtSettings>(configuration.GetSection("Jwt"));`
- Resolver `JwtSettings` ao montar `TokenValidationParameters` (via `BuildServiceProvider().GetRequiredService<IOptions<JwtSettings>>().Value` — ou `configuration.GetSection("Jwt").Get<JwtSettings>()` direto).

Em `TokenGenerationService` consumir `IOptions<JwtSettings>` no construtor.

Remover: `src/Edoha.Infraestructure/Constants/JwtConfig.cs`.
Atualizar `appsettings.json` para incluir `"RefreshExpiresInDays": 7`.

Corrigir o valor de `Jwt:Key` em `appsettings.json` retirando as **aspas duplas internas** (`"\"S3nh4...\""` → `"S3nh4..."`). Em produção, vir do Secrets Manager (ver task 01).

### 2.2 Tornar autenticação obrigatória por padrão

Em `Program.cs`, adicionar **policy default** que exige usuário autenticado:

```
builder.Services.AddAuthorization(options => {
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
```

(Note que `JwtInjection.AddJwt` já chama `AddAuthorization` para registrar `"PermissionPolicy"`. Unificar essa configuração — não chamar `AddAuthorization` duas vezes; mover o `FallbackPolicy` para dentro do mesmo `AddAuthorization` em `JwtInjection`.)

### 2.3 Marcar endpoints públicos com `[AllowAnonymous]`

Anotar:
- `AuthController` (classe inteira **ou** apenas o `Autenticate` POST `/auth`).
- `Autenticate` em `/auth/refresh` (criada no passo 2.5).
- Qualquer healthcheck (já que `MapHealthChecks` da task 01 ignora authorization se não houver policy aplicada — confirmar com `RequireHost`/`AllowAnonymousAttribute`).
- **Não** marcar `UserController.Create` (cadastro) como anônimo — a regra de negócio do projeto é admin criando usuários. Caso seja necessário público, abrir como exceção justificada.

### 2.4 Aplicar `[Authorize]` em controllers que precisam de validação fina

Aplicar `[Authorize(Policy = "PermissionPolicy")]` nos controllers que devem passar pelo `PermissionHandler`. A regra do handler hoje:
- Pega nome do controller (`page`) + método HTTP (`action`) + `idUser` da claim `NameIdentifier`.
- Consulta `UserPermissionService.GetUserActionByPageName`.

Aplicar nos controllers de domínio: `InstitutionController`, `LotteryController`, `TicketbookController`, `TicketController`, `UserController`, `UserPermissionController`, `UserInstitutionController`, `PermissionController`, `PageController`, `ActionController`, `UserTypeController`, `StatusTicketbookController`, `TableConfigurationController`.

> O `FallbackPolicy` do passo 2.2 cobre os controllers que **não** receberem `[Authorize(Policy=...)]`, garantindo que fiquem ao menos autenticados.

### 2.5 Implementar fluxo de refresh token

#### Endpoint novo: `POST /auth/refresh`

Body: `{ "accessToken": string, "refreshToken": string }` — o cliente manda o access expirado + refresh.

Fluxo no `AuthService.Refresh`:
1. Validar `accessToken` **ignorando expiração** (usar `TokenValidationParameters { ValidateLifetime = false }` em uma instância separada). Extrair `idUser` da claim.
2. Buscar `Login` ativo por `idUser` + `RefreshTokenHash` (comparar **hash** — ver 2.6).
3. Verificar `ExpiresAt > UtcNow` e `Revoked = false`.
4. **Rotacionar**: revogar o atual, gerar novo access + novo refresh, persistir o novo (com hash) e devolver.
5. Em caso de falha em qualquer etapa: `UnauthorizedAccessException`.

Adicionar em `LoginRepository`:
- `SelectActiveByUserAndRefreshHash(Guid idUser, string refreshHash)`
- `RevokeById(Guid id, DateTime revokedAt)`
- `Insert` (já existe via base) com `ExpiresAt` setado a partir de `JwtSettings.RefreshExpiresInDays`.

#### Endpoint novo: `POST /auth/logout`

`[Authorize]`. Recebe `{ "refreshToken": string }`. Marca o `Login` correspondente como `Revoked = true, RevokedAt = UtcNow`.

### 2.6 Hashear o refresh token antes de persistir

Hoje `AuthService.InsertLoginInformation` salva o refresh token em **claro**. Em `Login.RefreshTokenHash` espera-se hash.

- Adicionar `string HashRefreshToken(string raw)` em `Crypto` (HMAC-SHA256 com chave derivada da `JwtSettings.Key` ou SHA-256 simples — o nome da coluna sugere apenas hash; SHA-256 simples é suficiente para refresh tokens aleatórios).
- `InsertLoginInformation` deve gerar o refresh, **hashear**, persistir o hash, e retornar o **valor cru** ao chamador para entregar no response.
- `AuthResponse` precisa de `RefreshToken` (cru) — ver 2.7.

### 2.7 Atualizar `AuthResponse` e `AuthController`

`AuthResponse` ganha campos:
```
public string RefreshToken { get; set; } = "";
public DateTime AccessTokenExpiresAt { get; set; }
```
`AuthController.Autenticate` retorna o `AuthResponse` completo. Passa a usar `[AllowAnonymous]`.

### 2.8 Enriquecer claims do access token

Em `TokenGenerationService.GenerateToken`, adicionar:
- `ClaimTypes.NameIdentifier = user.Id`
- `ClaimTypes.Name = user.Nickname`
- Custom: `"institutions" = JSON com lista de Ids` ou várias claims `"institution"` com cada Guid (escolher uma das duas; preferir múltiplas claims para que `PermissionHandler` consiga ler sem JSON parse).

> A claim de instituição não é estritamente necessária para o `PermissionHandler` atual (ele só usa `NameIdentifier`), mas será útil para limitar `idInstitution` em `LotteryController` etc. Documentar como `// TODO`.

### 2.9 Corrigir `AuthService.Autenticate`

Hoje:
```
if (user.Institutions is null) {
    throw new CannotUnloadAppDomainException(...)
}
```
Trocar por uma exception adequada (`UnauthorizedAccessException` ou `RequestValidationException` com mensagem). Esta correção é mencionada também na task 08.

### 2.10 Garantir CORS expõe `Authorization` header

`Program.cs`:
```
policy.WithOrigins(...)
      .AllowAnyHeader()
      .AllowAnyMethod()
      .WithExposedHeaders("Authorization");
```

### 2.11 Swagger — Bearer já está em `JwtInjection`, só validar

Atualmente `JwtInjection` faz `services.AddSwaggerGen(c => { ... AddSecurityDefinition("Bearer") ... })`. **Mas** `Program.cs` também chama `services.AddSwaggerGen()` depois — a configuração pode estar sendo sobrescrita. Validar pelos logs do Swagger UI; se necessário, mover toda a config Swagger para um único lugar (preferência: `JwtInjection` ou um novo `SwaggerInjection`).

---

## Testes manuais sugeridos

1. `POST /auth` com credenciais válidas → 200 com `accessToken`, `refreshToken`, `idUser`, `institutions`.
2. `GET /institution` sem header `Authorization` → 401 com JSON `{ "message": "Token ausente ou inválido." }`.
3. `GET /institution` com `Authorization: Bearer <accessToken>` → 200.
4. Esperar expirar (ou setar `ExpiresInMinutes=1`), `POST /auth/refresh` com par válido → 200, novo par.
5. Repetir refresh com o **antigo** → 401 (foi rotacionado).
6. `POST /auth/logout` → próximas tentativas com aquele refresh devolvem 401.

---

## Critérios de aceitação

- [ ] `JwtConfig.cs` removido; chave/issuer/audience vêm de `JwtSettings`/`IConfiguration`.
- [ ] `Program.cs` tem `FallbackPolicy` exigindo usuário autenticado.
- [ ] `AuthController` tem `[AllowAnonymous]` no login (e refresh).
- [ ] Todos os outros controllers têm `[Authorize]` (ou herdam de uma `BaseController` com `[Authorize]`, ver task 03).
- [ ] `POST /auth/refresh` e `POST /auth/logout` existem e funcionam.
- [ ] `Login.RefreshTokenHash` armazena hash, não valor cru.
- [ ] `AuthResponse` inclui `RefreshToken` cru e `AccessTokenExpiresAt`.
- [ ] Substituída `CannotUnloadAppDomainException` por exceção semântica.
- [ ] Build OK (`dotnet build src/Edoha.sln`) e Swagger continua subindo com cadeado de Bearer.
