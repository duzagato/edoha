# 01 — Preparar API para deploy na AWS (ECS Fargate)

> **Dependência:** Nenhuma. Pode ser executada em paralelo com qualquer outra task.
> **Complexidade:** 🟡 Média
> **Tempo estimado de execução pela IA:** Curto (criação de arquivos de infra + ajustes pontuais em `Program.cs` / `appsettings.json` / `ServiceCollection.cs`).

---

## Objetivo

Deixar a API pronta para rodar como **container** num cluster **ECS com Fargate**, com:
- imagem Docker reproduzível
- configuração via **variáveis de ambiente** (12-factor)
- **healthcheck** HTTP funcional
- pipeline opcional de build/push para ECR
- arquivo de **task definition** modelo

Esta task **não** envolve refatoração de domínio. Foca exclusivamente em build, runtime e configuração.

---

## Itens de trabalho (em ordem)

### 1.1 Criar `Dockerfile` multi-stage na raiz do repositório

Caminho: `/Dockerfile`

Requisitos:
- Imagem base de build: `mcr.microsoft.com/dotnet/sdk:8.0`.
- Imagem base de runtime: `mcr.microsoft.com/dotnet/aspnet:8.0`.
- Restaurar com cache (copiar somente `*.csproj` + `*.sln` antes do `dotnet restore`).
- Publicar `src/Edoha.Application/Edoha.Application.csproj` em `Release` para `/app/publish`.
- Imagem final deve:
  - Expor a porta **8080** (HTTP, sem HTTPS dentro do container — TLS termina no ALB).
  - Definir `ASPNETCORE_URLS=http://+:8080`.
  - Definir `ASPNETCORE_ENVIRONMENT=Production` por padrão.
  - Rodar como **usuário não-root** (ex.: `USER app` da imagem aspnet:8.0 8080).
  - Entrypoint: `dotnet Edoha.Application.dll`.

### 1.2 Criar `.dockerignore` na raiz

Ignorar: `bin/`, `obj/`, `**/.vs/`, `**/.vscode/`, `**/.idea/`, `*.user`, `*.suo`, `*.log`, `*.binlog`, `**/appsettings.Development.json`, `.git`, `backlog/`, `**/TestResults/`, `Dockerfile`, `.github/`.

### 1.3 Remover `app.UseHttpsRedirection()` em produção

Arquivo: `src/Edoha.Application/Program.cs`, linha que faz `app.UseHttpsRedirection();`.

Motivo: no Fargate atrás de ALB com TLS, o ALB termina TLS e encaminha HTTP para o container. `UseHttpsRedirection` causaria loop de redirect / 307. Manter apenas em `Development`:

```
if (app.Environment.IsDevelopment()) {
    app.UseHttpsRedirection();
}
```

### 1.4 Adicionar healthcheck

Em `Program.cs`:
- `builder.Services.AddHealthChecks();` (e se quiser checar Postgres adicione `AddNpgSql(<connStr>)` — opcional, requer pacote `AspNetCore.HealthChecks.NpgSql`).
- `app.MapHealthChecks("/health");` antes de `app.MapControllers();`.

A task definition (passo 1.8) usará `/health` no `healthCheck.command` do container.

### 1.5 Externalizar configurações em variáveis de ambiente

#### `appsettings.json` (limpar)
- Remover a senha real de `ConnectionStrings:Default`.
- Substituir por valor de placeholder claro (ex.: `"Default": ""`) e comentar no README que `ConnectionStrings__Default` deve ser provida por env var.
- Remover `Jwt:Key` real (não comitar segredos). Manter apenas a estrutura:
  ```
  "Jwt": { "Issuer": "EdohaIssuer", "Audience": "EdohaAudience", "ExpiresInMinutes": 60 }
  ```

#### `JwtConfig.cs` (`src/Edoha.Infraestructure/Constants/JwtConfig.cs`)
- **Eliminar a classe `static`** (ou reduzir a um POCO `JwtSettings` com props read-only) e **deixar de usar `JwtConfig` em `TokenGenerationService`**. Consumir `IOptions<JwtSettings>` injetado, populado de `IConfiguration.GetSection("Jwt")`.
- Esta mudança é **pré-requisito** para que `appsettings.json`/env vars sejam a única fonte da verdade.

> Observação: parte desta refatoração pode também ser endereçada na task **02 (Auth)**. Se a task 02 for executada antes, esta sub-task vira no-op — apenas confirmar.

#### Mapeamento esperado em ECS task definition:
```
ConnectionStrings__Default = "Host=...;Port=5432;Database=edoha;Username=...;Password=...;SslMode=Require"
Jwt__Key                   = (do AWS Secrets Manager)
Jwt__Issuer                = EdohaIssuer
Jwt__Audience              = EdohaAudience
Jwt__ExpiresInMinutes      = 60
ASPNETCORE_ENVIRONMENT     = Production
ASPNETCORE_URLS            = http://+:8080
```

### 1.6 CORS configurável

Hoje em `Program.cs`:
```
policy.WithOrigins("http://localhost:4200")
```
Trocar por leitura de `Cors:AllowedOrigins` (array em `appsettings.json` / env var `Cors__AllowedOrigins__0` etc.). Adicionar fallback explícito vazio em produção (não permitir tudo).

### 1.7 Logs estruturados aderentes a CloudWatch

`Program.cs` já usa Serilog `WriteTo.Console`. Garantir formato **JSON** quando em produção:
- Adicionar pacote `Serilog.Formatting.Compact`.
- Em produção, `WriteTo.Console(new RenderedCompactJsonFormatter())`. Em dev manter o template legível atual.

CloudWatch coleta `stdout`/`stderr` automaticamente via `awslogs` driver (configurado na task definition).

### 1.8 Criar `deploy/ecs-task-definition.json` (modelo)

Caminho: `/deploy/ecs-task-definition.json`

Conteúdo modelo (placeholders entre `<>` para o usuário substituir/parametrizar via Terraform/CDK):

```json
{
  "family": "edoha-api",
  "networkMode": "awsvpc",
  "requiresCompatibilities": ["FARGATE"],
  "cpu": "512",
  "memory": "1024",
  "executionRoleArn": "<arn-da-role-de-execucao>",
  "taskRoleArn": "<arn-da-role-da-tarefa>",
  "containerDefinitions": [
    {
      "name": "edoha-api",
      "image": "<conta>.dkr.ecr.<regiao>.amazonaws.com/edoha-api:<tag>",
      "essential": true,
      "portMappings": [{ "containerPort": 8080, "protocol": "tcp" }],
      "environment": [
        { "name": "ASPNETCORE_ENVIRONMENT", "value": "Production" },
        { "name": "ASPNETCORE_URLS",        "value": "http://+:8080" }
      ],
      "secrets": [
        { "name": "ConnectionStrings__Default", "valueFrom": "<arn-do-secret-pg>" },
        { "name": "Jwt__Key",                   "valueFrom": "<arn-do-secret-jwt>" }
      ],
      "healthCheck": {
        "command": ["CMD-SHELL", "curl -fsS http://localhost:8080/health || exit 1"],
        "interval": 30, "timeout": 5, "retries": 3, "startPeriod": 30
      },
      "logConfiguration": {
        "logDriver": "awslogs",
        "options": {
          "awslogs-group": "/ecs/edoha-api",
          "awslogs-region": "<regiao>",
          "awslogs-stream-prefix": "edoha"
        }
      }
    }
  ]
}
```

> Observação: `curl` não está presente na imagem `aspnet:8.0` por padrão. Alternativa: usar o healthcheck nativo do ALB (Target Group `/health`) e remover `healthCheck` do container, **ou** adicionar `apt-get install -y curl` no estágio runtime do Dockerfile, **ou** trocar por um pequeno binário .NET. Decisão recomendada: **remover do container e usar Target Group**.

### 1.9 Criar workflow CI: build + push para ECR

Caminho: `.github/workflows/deploy-ecr.yml`

Gatilhos: push em `main` (ou tag `v*`).
Steps:
1. `actions/checkout@v4`
2. `aws-actions/configure-aws-credentials@v4` usando OIDC (`role-to-assume`).
3. `aws-actions/amazon-ecr-login@v2`.
4. `docker build -t $ECR/edoha-api:$GITHUB_SHA -t $ECR/edoha-api:latest .`
5. `docker push` ambas tags.
6. (Opcional) `aws-actions/amazon-ecs-render-task-definition@v1` + `amazon-ecs-deploy-task-definition@v2` para deploy automático.

Variáveis necessárias (Repository secrets/variables):
- `AWS_REGION`, `AWS_ROLE_TO_ASSUME`, `ECR_REPOSITORY=edoha-api`, `ECS_CLUSTER`, `ECS_SERVICE`.

### 1.10 README — seção “Deploy AWS”

Adicionar seção curta com:
- Como buildar localmente: `docker build -t edoha-api:dev .` e rodar passando env vars.
- Lista das variáveis de ambiente esperadas (mesma do passo 1.5).
- Que `/health` é o endpoint de healthcheck.
- Que TLS termina no ALB.

---

## Critérios de aceitação

- [ ] `docker build .` na raiz produz imagem que sobe e responde 200 em `GET /health`.
- [ ] Container roda como usuário não-root (`whoami` ≠ root).
- [ ] Nenhum segredo em `appsettings.json` no repositório (chave JWT real removida, senha de banco removida).
- [ ] `JwtConfig` estática deixou de ser usada para ler chave/issuer/audience (ou foi removida).
- [ ] CORS lê origens do `IConfiguration`.
- [ ] Workflow `.github/workflows/deploy-ecr.yml` existe e tem `dotnet build` + `docker build` + `docker push` lógicos.
- [ ] `deploy/ecs-task-definition.json` existe com healthcheck e secrets via `secretsmanager`.
- [ ] README atualizado com instruções de build e variáveis de ambiente.

---

## Notas para a IA executora

- Não introduzir `Microsoft.AspNetCore.HttpsRedirection` em produção, mas **manter** middlewares que já existem (Auth, CORS, Controllers) na ordem atual.
- Não alterar `appsettings.Development.json` — ele é local e está no `.gitignore` (apesar de já commitado, não mexer aqui — endereçado na task 08).
- Se for reaproveitar o `JwtConfig` por já estar usado em `TokenGenerationService`, **deixe um TODO claro** apontando para a task 02 e mantenha o comportamento atual lendo agora de `IConfiguration` (sem quebrar o fluxo de geração de token).
- Não remover o pacote `Microsoft.Data.SqlClient` aqui (a remoção é parte da task 08).
