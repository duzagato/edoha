# Edoha API

API REST em .NET 8 para gestão de rifas e loterias.

---

## Deploy AWS (Lambda + API Gateway)

A API é implantada como **AWS Lambda com Container Image**, exposta via **API Gateway HTTP API**. O TLS termina no API Gateway — a função Lambda recebe HTTP.

### Rodar localmente (sem Docker)

```bash
dotnet run --project src/Edoha.Application/Edoha.Application.csproj
```

Swagger disponível em `https://localhost:<port>/swagger` apenas em `Development`.

### Build local com Docker

```bash
docker build -t edoha-api:dev .
```

> A imagem usa `public.ecr.aws/lambda/dotnet:8` como base de runtime. Para testes locais fora do Lambda, prefira `dotnet run`.

### Variáveis de ambiente obrigatórias

| Variável | Descrição |
|---|---|
| `ConnectionStrings__Default` | Connection string PostgreSQL (ex: `Host=...;Database=...;Username=...;Password=...;SslMode=Require`) |
| `Jwt__Key` | Chave secreta JWT (mínimo 32 caracteres) |
| `Jwt__Issuer` | Issuer do JWT (padrão: `EdohaIssuer`) |
| `Jwt__Audience` | Audience do JWT (padrão: `EdohaAudience`) |
| `Jwt__ExpiresInMinutes` | Expiração do token em minutos (padrão: `60`) |
| `Cors__AllowedOrigins__0` | Primeira origem permitida no CORS (ex: `https://meuapp.com`) |
| `ASPNETCORE_ENVIRONMENT` | Ambiente (`Production` no Lambda) |

Em produção, `Jwt__Key` e `ConnectionStrings__Default` devem ser configurados nas variáveis de ambiente da função Lambda via **AWS Secrets Manager** ou diretamente no console AWS.

> **Não usar** `ASPNETCORE_URLS` — o adaptador Lambda gerencia o binding diretamente.

### Healthcheck

`GET /health` → 200 OK quando a API está no ar. Usado para Route 53 Health Checks ou CloudWatch Synthetics.

### Deploy manual

Pré-requisito: credenciais AWS configuradas com permissão de push para ECR e atualização de função Lambda.

```bash
# 1. Autenticar no ECR
aws ecr get-login-password --region <regiao> | \
  docker login --username AWS --password-stdin <conta>.dkr.ecr.<regiao>.amazonaws.com

# 2. Build e push da imagem
docker build -t <conta>.dkr.ecr.<regiao>.amazonaws.com/edoha-api:latest .
docker push <conta>.dkr.ecr.<regiao>.amazonaws.com/edoha-api:latest

# 3. Atualizar a função Lambda
aws lambda update-function-code \
  --function-name edoha-api \
  --image-uri <conta>.dkr.ecr.<regiao>.amazonaws.com/edoha-api:latest \
  --region <regiao>
```

### Deploy via SAM

```bash
# Build e deploy (primeira vez, interativo)
sam build --template deploy/template.yaml
sam deploy --guided

# Deploys subsequentes (usa samconfig.toml gerado)
sam build --template deploy/template.yaml && sam deploy
```

### CI/CD

O workflow `.github/workflows/deploy-lambda.yml` é acionado em push para `main` (ou tags `v*`) e:
1. Autentica na AWS via OIDC (sem credenciais de longa duração).
2. Faz build e push da imagem para o ECR com tags `$GITHUB_SHA` e `latest`.
3. Aciona `aws lambda update-function-code` para deploy automático.

**Secrets/Variables necessários no repositório GitHub:**

| Nome | Tipo | Descrição |
|---|---|---|
| `AWS_ROLE_TO_ASSUME` | Secret | ARN da IAM Role com permissão de push para ECR e deploy no Lambda |
| `AWS_REGION` | Variable | Região AWS (ex: `sa-east-1`) |
| `ECR_REPOSITORY` | Variable | Nome do repositório ECR (padrão: `edoha-api`) |
| `LAMBDA_FUNCTION_NAME` | Variable | Nome da função Lambda (padrão: `edoha-api`) |

### VPC (se o banco estiver em rede privada)

Se o PostgreSQL estiver em uma VPC privada (ex: RDS em subnet privada), a função Lambda precisa ser configurada na mesma VPC. Ver campo `VpcConfig` comentado em `deploy/template.yaml`.
