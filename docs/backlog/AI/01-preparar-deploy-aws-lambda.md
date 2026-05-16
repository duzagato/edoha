# 01 — Preparar API para deploy na AWS (Lambda + API Gateway via Container Image)

> **Dependência:** Nenhuma. Pode ser executada em paralelo com qualquer outra task.
> **Complexidade:** 🟡 Média
> **Tempo estimado de execução pela IA:** Curto (ajuste de Dockerfile, `Program.cs`, arquivos de infra e CI/CD).

---

## Objetivo

Deixar a API pronta para rodar como **AWS Lambda com Container Image** via **API Gateway HTTP API**, com:
- imagem Docker adaptada para o runtime do Lambda (`public.ecr.aws/lambda/dotnet:8`)
- integração do ASP.NET Core com Lambda via `Amazon.Lambda.AspNetCoreServer.Hosting`
- configuração via **variáveis de ambiente** (12-factor)
- endpoint de **healthcheck** HTTP funcional
- template SAM (`deploy/template.yaml`) modelo para infraestrutura (API Gateway + Lambda)
- pipeline de CI/CD via GitHub Actions (build/push para ECR + `lambda update-function-code`)

Esta task **não** envolve refatoração de domínio. Foca exclusivamente em build, runtime e configuração.

> **Contexto de decisão:** escolhemos Lambda com Container Image em vez de ECS Fargate. O fluxo de build/push para ECR se mantém; o que muda é o destino (Lambda função, não ECS Service) e a imagem base do Dockerfile.

---

## Itens de trabalho (em ordem)

### 1.1 Adicionar pacote `Amazon.Lambda.AspNetCoreServer.Hosting`

Arquivo: `src/Edoha.Application/Edoha.Application.csproj`

Adicionar:
```xml
<PackageReference Include="Amazon.Lambda.AspNetCoreServer.Hosting" Version="9.*" />
```

### 1.2 Adaptar `Program.cs` para Lambda

Adicionar **antes** de `builder.Build()`:

```csharp
builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);
```

Isso habilita execução tanto como Lambda (quando invocado pelo API Gateway) quanto como app ASP.NET Core normal (quando rodado com `dotnet run`). O adaptador é transparente para o restante do pipeline — rotas, middlewares e controllers não precisam mudar.

### 1.3 Atualizar `Dockerfile` para Lambda Container Image

A imagem base de runtime muda de `mcr.microsoft.com/dotnet/aspnet:8.0` para `public.ecr.aws/lambda/dotnet:8`.

Caminho: `/Dockerfile`

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/Edoha.sln .
COPY src/Edoha.Application/Edoha.Application.csproj Edoha.Application/
COPY src/Edoha.Domain/Edoha.Domain.csproj Edoha.Domain/
COPY src/Edoha.Infraestructure/Edoha.Infraestructure.csproj Edoha.Infraestructure/
COPY src/Edoha.Shared/Edoha.Shared.csproj Edoha.Shared/
RUN dotnet restore Edoha.sln

COPY src/ .
RUN dotnet publish Edoha.Application/Edoha.Application.csproj \
    -c Release -o /app/publish --no-restore

# Runtime stage — imagem base Lambda para .NET 8
FROM public.ecr.aws/lambda/dotnet:8 AS runtime
COPY --from=build /app/publish ${LAMBDA_TASK_ROOT}

# Handler: nome do assembly (sem extensão)
CMD ["Edoha.Application"]
```

Diferenças em relação ao Dockerfile ECS:
- **Sem** `EXPOSE 8080` — Lambda não expõe porta TCP diretamente.
- **Sem** `ENV ASPNETCORE_URLS` — o adaptador Lambda gerencia o binding.
- **Sem** `USER app` — a imagem base Lambda já roda com usuário não-root (`sbx_user1051`).
- `${LAMBDA_TASK_ROOT}` é a variável de ambiente da imagem Lambda (`/var/task`).
- `CMD` define o handler da função (nome do assembly).

### 1.4 Remover `app.UseHttpsRedirection()` em produção

Arquivo: `src/Edoha.Application/Program.cs`.

Motivo: no Lambda atrás de API Gateway HTTP API, o API Gateway termina TLS e encaminha HTTP para a função. `UseHttpsRedirection` causaria loop de redirect. Manter apenas em `Development`:

```csharp
if (app.Environment.IsDevelopment()) {
    app.UseHttpsRedirection();
}
```

### 1.5 Adicionar healthcheck

Em `Program.cs`:
```csharp
builder.Services.AddHealthChecks();
// ...
app.MapHealthChecks("/health");  // antes de app.MapControllers()
```

O endpoint `/health` serve para validação manual, Route 53 Health Checks e CloudWatch Synthetics.

> Lambda Container Image não precisa de container healthcheck como ECS. O API Gateway roteia diretamente para a função — não há configuração extra de healthcheck na infraestrutura.

### 1.6 Externalizar configurações em variáveis de ambiente

#### `appsettings.json` (limpar)
- Remover a senha real de `ConnectionStrings:Default`. Substituir por placeholder vazio: `"Default": ""`.
- Remover `Jwt:Key` real. Manter apenas estrutura:
  ```json
  "Jwt": { "Issuer": "EdohaIssuer", "Audience": "EdohaAudience", "ExpiresInMinutes": 60 }
  ```

#### `JwtConfig.cs`
- Eliminar a classe `static` e substituir por `JwtSettings` POCO injetado via `IOptions<JwtSettings>`.
- Consumir `IConfiguration.GetSection("Jwt")`.

> Esta refatoração também é endereçada na task **02 (Auth)**. Se 02 já foi executada, sub-task é no-op.

#### Mapeamento esperado nas variáveis de ambiente da função Lambda:
```
ConnectionStrings__Default  = "Host=...;Port=5432;Database=edoha;Username=...;Password=...;SslMode=Require"
Jwt__Key                    = (do AWS Secrets Manager)
Jwt__Issuer                 = EdohaIssuer
Jwt__Audience               = EdohaAudience
Jwt__ExpiresInMinutes       = 60
ASPNETCORE_ENVIRONMENT      = Production
```

> **Não** usar `ASPNETCORE_URLS` — o adaptador Lambda ignora essa variável.

### 1.7 CORS configurável

Trocar em `Program.cs`:
```csharp
policy.WithOrigins("http://localhost:4200")
```
Por leitura de `Cors:AllowedOrigins` (array em `appsettings.json` / env var `Cors__AllowedOrigins__0` etc.). Adicionar fallback explícito vazio em produção.

### 1.8 Logs estruturados aderentes a CloudWatch

Lambda captura automaticamente `stdout`/`stderr` e envia ao CloudWatch Logs — não é necessário configurar log driver como no ECS.

`Program.cs` já usa Serilog com `WriteTo.Console`. Garantir formato **JSON** em produção:
- Adicionar pacote `Serilog.Formatting.Compact`.
- Em produção: `WriteTo.Console(new RenderedCompactJsonFormatter())`. Em dev: manter template legível.

### 1.9 Criar `deploy/template.yaml` (SAM — modelo)

Caminho: `/deploy/template.yaml`

```yaml
AWSTemplateFormatVersion: '2010-09-09'
Transform: AWS::Serverless-2016-10-31
Description: Edoha API — AWS Lambda Container Image + API Gateway HTTP API

Globals:
  Function:
    MemorySize: 512
    Timeout: 30
    Environment:
      Variables:
        ASPNETCORE_ENVIRONMENT: Production
        Jwt__Issuer: EdohaIssuer
        Jwt__Audience: EdohaAudience
        Jwt__ExpiresInMinutes: "60"

Resources:
  EdohaApi:
    Type: AWS::Serverless::HttpApi
    Properties:
      Description: API Gateway HTTP API — Edoha

  EdohaFunction:
    Type: AWS::Serverless::Function
    Properties:
      FunctionName: edoha-api
      PackageType: Image
      ImageUri: <conta>.dkr.ecr.<regiao>.amazonaws.com/edoha-api:<tag>
      Role: !Sub arn:aws:iam::${AWS::AccountId}:role/<nome-da-role-lambda>
      Environment:
        Variables:
          ConnectionStrings__Default: !Sub "{{resolve:secretsmanager:<arn-do-secret-pg>}}"
          Jwt__Key: !Sub "{{resolve:secretsmanager:<arn-do-secret-jwt>}}"
      Events:
        ApiRoot:
          Type: HttpApi
          Properties:
            ApiId: !Ref EdohaApi
            Path: /
            Method: ANY
        ApiProxy:
          Type: HttpApi
          Properties:
            ApiId: !Ref EdohaApi
            Path: /{proxy+}
            Method: ANY
      # Se o banco estiver em VPC privada, descomentar:
      # VpcConfig:
      #   SubnetIds: [<subnet-id-1>, <subnet-id-2>]
      #   SecurityGroupIds: [<security-group-id>]
    Metadata:
      DockerTag: latest
      DockerContext: ../
      Dockerfile: Dockerfile

Outputs:
  ApiEndpoint:
    Description: URL do API Gateway HTTP API
    Value: !Sub https://${EdohaApi}.execute-api.${AWS::Region}.amazonaws.com
  FunctionArn:
    Description: ARN da função Lambda
    Value: !GetAtt EdohaFunction.Arn
```

> **Sobre secrets:** para passar segredos ao Lambda em runtime, configure as variáveis de ambiente da função diretamente com valores do Secrets Manager via AWS Console/CLI, ou use a **AWS Parameters and Secrets Lambda Extension** para resolução em tempo de inicialização.

### 1.10 Atualizar workflow CI: build/push ECR + deploy Lambda

Caminho: `.github/workflows/deploy-lambda.yml` (substituindo `deploy-ecr.yml`)

```yaml
name: Build, Push ECR e Deploy no Lambda

on:
  push:
    branches: [main]
    tags: ['v*']

permissions:
  id-token: write
  contents: read

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    env:
      ECR_REPOSITORY: ${{ vars.ECR_REPOSITORY || 'edoha-api' }}

    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Configurar credenciais AWS via OIDC
        uses: aws-actions/configure-aws-credentials@v4
        with:
          role-to-assume: ${{ secrets.AWS_ROLE_TO_ASSUME }}
          aws-region: ${{ vars.AWS_REGION }}

      - name: Login no Amazon ECR
        id: login-ecr
        uses: aws-actions/amazon-ecr-login@v2

      - name: Build e Push da imagem
        env:
          REGISTRY: ${{ steps.login-ecr.outputs.registry }}
          IMAGE_TAG: ${{ github.sha }}
        run: |
          docker build \
            -t $REGISTRY/$ECR_REPOSITORY:$IMAGE_TAG \
            -t $REGISTRY/$ECR_REPOSITORY:latest .
          docker push $REGISTRY/$ECR_REPOSITORY:$IMAGE_TAG
          docker push $REGISTRY/$ECR_REPOSITORY:latest

      - name: Deploy no Lambda
        if: github.ref == 'refs/heads/main'
        env:
          REGISTRY: ${{ steps.login-ecr.outputs.registry }}
          IMAGE_TAG: ${{ github.sha }}
        run: |
          aws lambda update-function-code \
            --function-name ${{ vars.LAMBDA_FUNCTION_NAME || 'edoha-api' }} \
            --image-uri $REGISTRY/$ECR_REPOSITORY:$IMAGE_TAG \
            --region ${{ vars.AWS_REGION }}
```

**Variáveis necessárias (comparação ECS → Lambda):**

| Variável | ECS | Lambda |
|---|---|---|
| `AWS_ROLE_TO_ASSUME` | ✅ mantém | ✅ mantém |
| `AWS_REGION` | ✅ mantém | ✅ mantém |
| `ECR_REPOSITORY` | ✅ mantém | ✅ mantém |
| `ECS_CLUSTER` | ✅ usava | ❌ remover |
| `ECS_SERVICE` | ✅ usava | ❌ remover |
| `LAMBDA_FUNCTION_NAME` | — | ✅ adicionar |

### 1.11 README — seção "Deploy AWS Lambda"

Adicionar seção com:
- Como rodar localmente: `dotnet run --project src/Edoha.Application/Edoha.Application.csproj`.
- Como buildar a imagem localmente: `docker build -t edoha-api:dev .`.
- Lista das variáveis de ambiente esperadas (passo 1.6).
- Como fazer deploy manual: `aws lambda update-function-code --function-name edoha-api --image-uri ...`.
- Como usar SAM: `sam build && sam deploy --guided`.
- Que TLS termina no API Gateway HTTP API.
- Que `/health` é o endpoint de healthcheck.
- Observação sobre VPC se o banco estiver em rede privada.

---

## Considerações sobre cold start

Lambda Container Image tem cold starts tipicamente de 2–5s (maior que ZIP por precisar carregar a imagem). Para mitigar:
- **MemorySize 512 MB ou maior**: mais memória = mais CPU alocada = inicialização mais rápida.
- **Provisioned Concurrency**: garante instâncias quentes (custo adicional).
- **Imagem enxuta**: o build multi-stage já garante que apenas o artefato publicado vai para o runtime.

---

## Consideração sobre VPC

Se o banco PostgreSQL (RDS) estiver em uma VPC privada, a função Lambda **precisa ser configurada na mesma VPC** (subnets privadas + security group permitindo acesso à porta 5432). Configurar no template SAM (campo `VpcConfig` já incluído comentado no passo 1.9) e ao criar/atualizar a função via console ou CLI.

---

## Critérios de aceitação

- [ ] `Amazon.Lambda.AspNetCoreServer.Hosting` adicionado ao `.csproj`.
- [ ] `Program.cs` chama `AddAWSLambdaHosting(LambdaEventSource.HttpApi)`.
- [ ] `Dockerfile` usa `public.ecr.aws/lambda/dotnet:8` como base de runtime.
- [ ] `app.UseHttpsRedirection()` condicionado a `Development`.
- [ ] `app.MapHealthChecks("/health")` registrado.
- [ ] Nenhum segredo em `appsettings.json` (chave JWT e senha de banco removidas).
- [ ] `JwtConfig` estática substituída por `IOptions<JwtSettings>` (ou task 02 já fez isso).
- [ ] CORS lê origens do `IConfiguration`.
- [ ] `deploy/template.yaml` (SAM) criado com API Gateway HTTP API + Lambda Container Image.
- [ ] Workflow `deploy-lambda.yml` criado com build + push ECR + `lambda update-function-code`.
- [ ] README atualizado com instruções de deploy Lambda e variáveis de ambiente.

---

## Notas para a IA executora

- `AddAWSLambdaHosting` é no-op quando a app roda localmente (sem variáveis do Lambda Runtime). `dotnet run` continua funcionando normalmente.
- **Não** adicionar `ASPNETCORE_URLS` — o adaptador Lambda não usa essa variável.
- Manter todos os middlewares existentes (Auth, CORS, Controllers) na ordem atual.
- Não alterar `appsettings.Development.json`.
- Não remover `Microsoft.Data.SqlClient` aqui — endereçado na task 08.
- Lambda tem timeout máximo de 15 minutos; 30s é suficiente para esta API REST.
- Logs no CloudWatch são automáticos — não configurar awslogs driver.
- Payload máximo do API Gateway HTTP API: **6 MB** por requisição. Para uploads grandes, considerar S3 presigned URLs futuramente.
