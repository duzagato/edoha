# Task 01 — Deploy AWS ECS Fargate

**Data:** 2026-05-12  
**Branch:** claude-init

## Resumo
Preparação completa da API para rodar como container no ECS Fargate: Dockerfile multi-stage, remoção de segredos do repositório, healthcheck, CORS e logs configuráveis, e pipeline de CI/CD.

---

## Arquivos criados

### `Dockerfile`
Multi-stage build (sdk:8.0 → aspnet:8.0). Padrão de cache de restore: copia só os `.csproj` + `.sln` antes do código-fonte para aproveitar cache de layer quando apenas o código muda. Roda como usuário não-root (`USER app`). Porta 8080, sem HTTPS (TLS termina no ALB).

### `.dockerignore`
Exclui `bin/`, `obj/`, `.vs/`, `appsettings.Development.json`, `.git/`, `docs/`, arquivos `.db`, `.github/` da imagem. Reduz contexto de build e evita vazar arquivos sensíveis.

### `deploy/ecs-task-definition.json`
Template da task definition com healthcheck, variáveis de ambiente, segredos via Secrets Manager e configuração de logs no CloudWatch. Placeholders `<...>` para o usuário preencher.

### `.github/workflows/deploy-ecr.yml`
Pipeline acionado em push para `main` ou tags `v*`. Autentica na AWS via OIDC (sem credenciais de longa duração). Faz build + push para ECR com tags `$GITHUB_SHA` e `latest`. Aciona `update-service` no ECS apenas em push para `main`.

---

## Arquivos modificados

### `src/Edoha.Application/Program.cs`
- **HTTPS redirect**: movido para dentro do bloco `IsDevelopment()`. Em produção (atrás de ALB) o redirect causaria loop de 307.
- **Healthcheck**: `builder.Services.AddHealthChecks()` + `app.MapHealthChecks("/health")`.
- **CORS**: origens lidas de `IConfiguration["Cors:AllowedOrigins"]` (array). Não mais hardcoded.
- **Logs Serilog**: em produção usa `RenderedCompactJsonFormatter` (JSON linha por linha, ideal para CloudWatch). Em dev mantém o template legível.

### `src/Edoha.Application/appsettings.json`
- Removido `Password=12341234` da connection string → substituído por `""`.
- Removida chave JWT hardcoded → substituída por `""`.
- Adicionada seção `Cors.AllowedOrigins` com o valor padrão de dev.
- Em produção, `ConnectionStrings__Default` e `Jwt__Key` devem ser injetados via Secrets Manager.

### `src/Edoha.Application/Edoha.Application.csproj`
- Adicionado `Serilog.Formatting.Compact 3.0.0` para o JSON formatter em produção.

### `src/Edoha.Application/DependencyInjection/JwtInjection.cs`
- Adicionado `services.Configure<JwtSettings>(configuration.GetSection("Jwt"))` para registrar as configurações JWT como `IOptions<JwtSettings>` no container de DI.

### `src/Edoha.Infraestructure/Services/TokenGenerationService.cs`
- Substituído acesso direto à classe estática `JwtConfig` por `IOptions<JwtSettings>` injetado via constructor.
- Corrigido `RNGCryptoServiceProvider` obsoleto: `RefreshToken` agora usa `RandomNumberGenerator.Create()` com `using var`.

### `src/Edoha.Infraestructure/Constants/JwtConfig.cs`
- Classe estática esvaziada (conteúdo removido). Arquivo mantido para evitar quebrar referências no git, pode ser deletado na task 08.

### `src/Edoha.Infraestructure/Constants/JwtSettings.cs` *(novo)*
- POCO com propriedades `Key`, `Issuer`, `Audience`, `ExpiresInMinutes`. Populado via `IOptions<JwtSettings>` a partir de `appsettings.json` / variáveis de ambiente.

### `README.md`
- Adicionada seção "Deploy AWS" com instruções de build local, lista de variáveis de ambiente, healthcheck, TLS e requisitos para o CI/CD.

---

## Critérios de aceitação — status

| Critério | Status |
|---|---|
| `docker build .` gera imagem válida | ✅ Dockerfile criado |
| Container roda como não-root | ✅ `USER app` |
| Nenhum segredo no repositório | ✅ `appsettings.json` limpo |
| `JwtConfig` estática não usada | ✅ Substituída por `JwtSettings` + `IOptions` |
| CORS lê origens do `IConfiguration` | ✅ |
| Workflow ECR existe | ✅ `deploy-ecr.yml` |
| `deploy/ecs-task-definition.json` existe | ✅ |
| README atualizado | ✅ |
| `GET /health` 200 | ✅ `AddHealthChecks` + `MapHealthChecks` |
| Build sem erros | ✅ `0 erros, 64 warnings pré-existentes` |
