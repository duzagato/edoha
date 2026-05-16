# Task 01 — Deploy AWS Lambda (Container Image + API Gateway)

**Data:** 2026-05-15  
**Branch:** claude-init

## Resumo

Preparação completa da API para rodar como **AWS Lambda com Container Image** exposta via **API Gateway HTTP API**: adaptador ASP.NET Core para Lambda, Dockerfile atualizado para a imagem base Lambda, remoção de segredos do repositório, template SAM de infraestrutura e pipeline CI/CD atualizado.

> Esta task substitui a versão anterior (ECS Fargate). Os arquivos de infra antigos (`deploy-ecr.yml`, `ecs-task-definition.json`) foram removidos e substituídos.

---

## Arquivos criados

### `aws-lambda-tools-defaults.json`
Arquivo de configuração para o CLI `dotnet lambda` e SAM. Define nome da função, região, memória, timeout e tipo de pacote (`image`). Placeholders `<...>` para o usuário preencher com valores reais.

### `deploy/template.yaml`
Template AWS SAM com API Gateway HTTP API + função Lambda Container Image. Inclui: variáveis de ambiente, referências a secrets via Secrets Manager (placeholders), configuração de VPC comentada para uso quando o banco estiver em rede privada.

### `.github/workflows/deploy-lambda.yml`
Pipeline acionado em push para `main` ou tags `v*`. Autentica na AWS via OIDC. Faz build + push para ECR com tags `$GITHUB_SHA` e `latest`. Aciona `aws lambda update-function-code --image-uri` para deploy (substituiu `aws ecs update-service`).

---

## Arquivos modificados

### `src/Edoha.Application/Edoha.Application.csproj`
- Adicionado pacote `Amazon.Lambda.AspNetCoreServer.Hosting 2.0.0` para integração do ASP.NET Core com o runtime Lambda.

### `src/Edoha.Application/Program.cs`
- Adicionado `using Amazon.Lambda.AspNetCoreServer.Hosting`.
- Adicionada chamada `builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi)` antes de `builder.Build()`. É no-op quando executado localmente via `dotnet run`.

> **Nota:** os demais itens da task (`AddHealthChecks`, CORS via `IConfiguration`, `UseHttpsRedirection` condicional, logs JSON em produção) já estavam implementados na versão anterior da task (ECS Fargate). Nenhuma alteração adicional necessária nesses pontos.

### `src/Edoha.Application/appsettings.json`
- Removida connection string real (Neon.tech com credenciais expostas) → substituída por `""`.
- Removida chave JWT hardcoded com aspas internas → substituída por `""`.
- Adicionada seção `Cors.AllowedOrigins` com array vazio (origens configuradas via variáveis de ambiente em produção).

### `Dockerfile`
- Imagem base de runtime alterada de `mcr.microsoft.com/dotnet/aspnet:8.0` para `public.ecr.aws/lambda/dotnet:8`.
- Removidos `EXPOSE 8080`, `ENV ASPNETCORE_URLS`, `USER app` (não aplicáveis no Lambda).
- Destino da cópia alterado para `${LAMBDA_TASK_ROOT}` (`/var/task`).
- Entrypoint alterado de `dotnet Edoha.Application.dll` para `CMD ["Edoha.Application"]` (formato handler Lambda).

### `README.md`
- Seção "Deploy AWS (ECS Fargate)" reescrita para "Deploy AWS (Lambda + API Gateway)".
- Removidas referências a `ASPNETCORE_URLS`, ALB, `ECS_CLUSTER`, `ECS_SERVICE`.
- Adicionadas instruções de deploy manual via ECR + `aws lambda update-function-code`.
- Adicionadas instruções de deploy via SAM (`sam build && sam deploy --guided`).
- Tabela de variáveis atualizada (adicionado `LAMBDA_FUNCTION_NAME`, removidas variáveis ECS).
- Nota sobre VPC para banco em rede privada.

### `.gitignore`
- Adicionado `samconfig.toml` (gerado por `sam deploy --guided`, pode conter IDs de conta AWS e região).

---

## Arquivos removidos

| Arquivo | Motivo |
|---|---|
| `deploy/ecs-task-definition.json` | Substituído por `deploy/template.yaml` (SAM) |
| `.github/workflows/deploy-ecr.yml` | Substituído por `deploy-lambda.yml` |
| `docs/backlog/AI/01-preparar-deploy-aws-ecs-fargate.md` | Substituído por `01-preparar-deploy-aws-lambda.md` |

---

## Critérios de aceitação — status

| Critério | Status |
|---|---|
| `Amazon.Lambda.AspNetCoreServer.Hosting` no `.csproj` | ✅ v2.0.0 adicionado |
| `Program.cs` chama `AddAWSLambdaHosting` | ✅ |
| `Dockerfile` usa base Lambda (`public.ecr.aws/lambda/dotnet:8`) | ✅ |
| `app.UseHttpsRedirection()` condicionado a Development | ✅ já estava |
| `app.MapHealthChecks("/health")` registrado | ✅ já estava |
| Nenhum segredo em `appsettings.json` | ✅ connection string e JWT key removidos |
| `JwtConfig` estática substituída por `IOptions<JwtSettings>` | ✅ já estava (task anterior) |
| CORS lê origens do `IConfiguration` | ✅ já estava |
| `deploy/template.yaml` (SAM) criado | ✅ |
| Workflow `deploy-lambda.yml` criado | ✅ |
| README atualizado | ✅ |
| `aws-lambda-tools-defaults.json` criado | ✅ |
| `.gitignore` com `samconfig.toml` | ✅ |
| Build sem erros | ✅ 0 erros, warnings pré-existentes |
