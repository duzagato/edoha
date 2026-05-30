# Deploy Manual Emergencial — AWS Lambda (ECR)

> **Contexto:** Deploy manual sem uso dos workflows do GitHub Actions. O repositório ECR já está criado.

---

## 1. Análise do Projeto

### 1.1 Dockerfile — Estado atual

O Dockerfile está **correto** para deploy Lambda via Container Image. Resumo do que foi validado:

| Item | Status | Observação |
|---|---|---|
| Imagem de build | ✅ | `mcr.microsoft.com/dotnet/sdk:8.0` |
| Imagem de runtime | ✅ | `public.ecr.aws/lambda/dotnet:8` (imagem Lambda oficial) |
| Cache de restore | ✅ | `.csproj` copiados antes do código para otimizar layers |
| Destino do publish | ✅ | `${LAMBDA_TASK_ROOT}` (`/var/task`) — diretório que o Lambda lê |
| Handler (CMD) | ✅ | `["Edoha.Application"]` — nome do assembly, correto para `AddAWSLambdaHosting` |
| Build mode | ✅ | Release com `--no-restore` |

### 1.2 .dockerignore — Estado atual

O `.dockerignore` está **correto**. Exclui:

- `bin/`, `obj/`, arquivos do IDE → correto, só o código entra
- `appsettings.Development.json` → **crítico**: impede que configurações locais vazem para produção
- `.git/`, `docs/`, arquivos de teste → correto
- `Dockerfile`, `.dockerignore`, `.github/` → correto

Nenhuma alteração necessária.

### 1.3 Configuração — Problemas Encontrados

#### 🔴 CRÍTICO — `ServiceInjection.cs`: region nula em produção

**Arquivo:** `src/Edoha.Application/DependencyInjection/ServiceInjection.cs`, linha 32

```csharp
var region = config["AWS:Region"]; // retorna null em produção
services.AddSingleton<IAmazonSecretsManager>(_ =>
    new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region))); // QUEBRA se null
```

**Por quê quebra:** `AWS:Region` está definido apenas em `appsettings.Development.json`, que o `.dockerignore` exclui corretamente da imagem. Em produção (Lambda), essa chave será `null`, e `RegionEndpoint.GetBySystemName(null)` lança exceção na inicialização da função.

**Fix necessário antes do build** (ver seção 2.1).

#### 🟡 IMPORTANTE — JWT desabilitado em `Program.cs`

**Arquivo:** `src/Edoha.Application/Program.cs`, linha 26

```csharp
//builder.Services.AddJwt(builder.Configuration);
```

A autenticação JWT está comentada. Rotas com `[Authorize]` **não vão funcionar** — retornarão 401 ou ignorarão a policy, dependendo da configuração. Se o objetivo do deploy emergencial é ter a API funcional com segurança, isso precisa ser avaliado.

#### 🟡 IMPORTANTE — Secrets Manager não está integrado ao fluxo de conexão

O código de `ISecretsManagerService` existe e está registrado no DI, mas **não é chamado em lugar nenhum** para popular a connection string. O fluxo atual é:

```
Lambda → lê env var ConnectionStrings__Default → passa direto para o Dapper
```

O `SecretsManagerService.GetConnectionStringAsync()` está implementado mas sem consumidor. A chave `SecretKeys.DatabaseConnection = "ConnectionString"` precisaria corresponder ao nome exato de um secret no Secrets Manager — mas como o código não é chamado, isso não impacta o deploy agora.

**Para este deploy manual:** configure a connection string diretamente como variável de ambiente na Lambda (ver seção 3.4). Não dependa do Secrets Manager agora.

#### 🟡 IMPORTANTE — CORS sem origens configuradas

**Arquivo:** `src/Edoha.Application/appsettings.json`

```json
"Cors": { "AllowedOrigins": [] }
```

Com o array vazio, a policy `AllowAngular` não chama `.WithOrigins()`, então qualquer requisição do browser será bloqueada pelo CORS. Configure via variável de ambiente na Lambda se houver frontend.

#### 🟡 — `aws-lambda-tools-defaults.json` com placeholders

**Arquivo:** `aws-lambda-tools-defaults.json`

```json
{
  "region": "<regiao>",
  "image-tag": "<conta>.dkr.ecr.<regiao>.amazonaws.com/edoha-api:latest"
}
```

Não impacta o deploy manual, mas deve ser atualizado com os valores reais para uso futuro com `dotnet lambda deploy-function`.

---

## 2. Correções Necessárias Antes do Build

### 2.1 Corrigir `ServiceInjection.cs` — Suporte a região via variável de ambiente

Abra `src/Edoha.Application/DependencyInjection/ServiceInjection.cs` e substitua:

```csharp
// ANTES
var region = config["AWS:Region"];

services.AddSingleton<IAmazonSecretsManager>(_ =>
    new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region)));
```

Por:

```csharp
// DEPOIS
var region = config["AWS:Region"]
    ?? Environment.GetEnvironmentVariable("AWS_REGION")
    ?? Environment.GetEnvironmentVariable("AWS_DEFAULT_REGION");

services.AddSingleton<IAmazonSecretsManager>(_ =>
    string.IsNullOrEmpty(region)
        ? new AmazonSecretsManagerClient()
        : new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region)));
```

**Por quê:** o Lambda injeta `AWS_REGION` automaticamente no ambiente de execução. Com o fallback, o SDK resolve a região sem precisar de configuração extra. Se `region` for nulo mesmo assim, o SDK usa a região padrão da conta/role.

---

## 3. Build e Push da Imagem para o ECR

### Pré-requisitos

- [AWS CLI v2](https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html) instalado e configurado com credenciais válidas
- Docker Desktop em execução
- Acesso ao repositório ECR já criado
- Variáveis substituídas: `<ACCOUNT_ID>` e `<REGION>` nos comandos abaixo

> **Dica:** Para descobrir o ID da conta, execute `aws sts get-caller-identity --query Account --output text`

### 3.1 Autenticar Docker no ECR

```bash
aws ecr get-login-password --region sa-east-1 | docker login \
  --username AWS \
  --password-stdin <ACCOUNT_ID>.dkr.ecr.sa-east-1.amazonaws.com
```

Saída esperada: `Login Succeeded`

### 3.2 Build da imagem

Execute a partir da **raiz do repositório** (onde está o `Dockerfile`):

```bash
docker build -t edoha-api:latest .
```

> O build pode demorar na primeira vez (download da imagem SDK ~800 MB). Nas seguintes, o cache de layers acelera.

Para verificar o resultado:
```bash
docker images edoha-api
```

### 3.3 Tag e Push para o ECR

```bash
# Tag com o repositório ECR
docker tag edoha-api:latest <ACCOUNT_ID>.dkr.ecr.sa-east-1.amazonaws.com/edoha-api:latest

# Push
docker push <ACCOUNT_ID>.dkr.ecr.sa-east-1.amazonaws.com/edoha-api:latest
```

Ao final, anote a URI completa com digest. Você pode copiá-la no console ECR → repositório → aba "Images".

---

## 4. Criação da Função Lambda no Console AWS

### 4.1 Criar a função

1. Acesse o console AWS → **Lambda** → **Create function**
2. Selecione: **Container image**
3. Preencha:
   - **Function name:** `edoha-api`
   - **Container image URI:** cole a URI do ECR (ex: `<ACCOUNT_ID>.dkr.ecr.sa-east-1.amazonaws.com/edoha-api:latest`)
   - Clique em **Browse image** para confirmar que a imagem está visível
4. **Architecture:** x86_64
5. **Execution role:**
   - Se ainda não existe: selecione **Create a new role with basic Lambda permissions**
   - Se já existe uma role: selecione **Use an existing role** e escolha a role adequada
6. Clique em **Create function**

### 4.2 Configurar memória e timeout

Após criar, vá em **Configuration** → **General configuration** → **Edit**:

| Parâmetro | Valor |
|---|---|
| Memory | **512 MB** |
| Timeout | **30 segundos** |
| Ephemeral storage | 512 MB (padrão) |

Clique em **Save**.

### 4.3 Permissões da Execution Role (IAM)

Vá em **Configuration** → **Permissions** → clique no nome da role.

A role precisa das seguintes permissões mínimas:

**Já incluídas na role básica:**
- `logs:CreateLogGroup`
- `logs:CreateLogStream`
- `logs:PutLogEvents`

**Adicionar manualmente (se for usar Secrets Manager futuramente):**
```json
{
  "Effect": "Allow",
  "Action": [
    "secretsmanager:GetSecretValue",
    "secretsmanager:DescribeSecret"
  ],
  "Resource": "arn:aws:secretsmanager:sa-east-1:<ACCOUNT_ID>:secret:edoha/*"
}
```

Para agora (deploy emergencial sem Secrets Manager), somente os logs já são suficientes.

### 4.4 Configurar Variáveis de Ambiente

No console da função → **Configuration** → **Environment variables** → **Edit** → **Add environment variable**:

| Chave | Valor | Obrigatório |
|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Production` | Sim |
| `ConnectionStrings__Default` | string de conexão PostgreSQL completa | **Sim — sem isso a API não conecta ao banco** |
| `AWS__Region` | `sa-east-1` | Sim (até o fix do item 2.1 ser deployado) |
| `Jwt__Issuer` | `EdohaIssuer` | Sim (quando JWT for reativado) |
| `Jwt__Audience` | `EdohaAudience` | Sim (quando JWT for reativado) |
| `Jwt__Key` | chave secreta JWT (mínimo 32 caracteres) | Sim (quando JWT for reativado) |
| `Jwt__ExpiresInMinutes` | `60` | Opcional |
| `Cors__AllowedOrigins__0` | URL do frontend (ex: `https://app.seudominio.com`) | Se houver frontend |

**Formato da connection string PostgreSQL:**
```
Host=<host>;Port=5432;Database=<db>;Username=<user>;Password=<senha>;SSL Mode=Require;Trust Server Certificate=true
```

> **Atenção:** Nunca commite credenciais no repositório. Use esta variável de ambiente temporariamente para o deploy emergencial. A integração definitiva é via Secrets Manager.

Clique em **Save**.

### 4.5 Configurar Trigger — API Gateway HTTP API

Para expor a Lambda via HTTP:

1. Na função → **Configuration** → **Triggers** → **Add trigger**
2. Selecione: **API Gateway**
3. Configuração:
   - **Intent:** Create a new API
   - **API type:** HTTP API
   - **Security:** Open *(ajuste conforme necessidade)*
4. Clique em **Add**

O console vai criar automaticamente:
- Uma HTTP API no API Gateway
- Uma rota `ANY /{proxy+}` apontando para a Lambda
- As permissões de invocação (`lambda:InvokeFunction`) para o API Gateway

A URL do endpoint ficará visível em **Configuration** → **Triggers** no formato:
```
https://<api-id>.execute-api.sa-east-1.amazonaws.com
```

### 4.6 Testar a função

**Via console (teste rápido):**
1. Aba **Test** → **Create new event**
2. Escolha template: `apigateway-http-api-proxy`
3. Clique em **Test**
4. Verifique nos logs se a aplicação inicializou sem erros

**Via HTTP (após configurar o trigger):**
```bash
curl https://<api-id>.execute-api.sa-east-1.amazonaws.com/health
```

Resposta esperada: `Healthy` com HTTP 200.

---

## 5. Atualizar a Imagem (Deploys Subsequentes)

Para deploys futuros sem CI/CD:

```bash
# 1. Rebuild
docker build -t edoha-api:latest .

# 2. Re-tag e push
docker tag edoha-api:latest <ACCOUNT_ID>.dkr.ecr.sa-east-1.amazonaws.com/edoha-api:latest
docker push <ACCOUNT_ID>.dkr.ecr.sa-east-1.amazonaws.com/edoha-api:latest

# 3. Forçar atualização da Lambda
aws lambda update-function-code \
  --function-name edoha-api \
  --image-uri <ACCOUNT_ID>.dkr.ecr.sa-east-1.amazonaws.com/edoha-api:latest \
  --region sa-east-1
```

---

## 6. Checklist Final

- [ ] Correção de `ServiceInjection.cs` feita (item 2.1)
- [ ] Docker autenticado no ECR
- [ ] Imagem buildada sem erros
- [ ] Push para ECR concluído
- [ ] Função Lambda `edoha-api` criada com Container Image
- [ ] Memória: 512 MB, Timeout: 30s configurados
- [ ] Variável `ConnectionStrings__Default` configurada na Lambda
- [ ] Variável `ASPNETCORE_ENVIRONMENT=Production` configurada
- [ ] Variável `AWS__Region=sa-east-1` configurada
- [ ] Trigger API Gateway HTTP API criado
- [ ] Endpoint `/health` retornando 200
