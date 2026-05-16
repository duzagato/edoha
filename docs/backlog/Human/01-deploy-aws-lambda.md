# Human Task 01 — Deploy AWS Lambda (Container Image)

**Relacionado a:** `docs/backlog/AI/01-preparar-deploy-aws-lambda.md`  
**Data de geração:** 2026-05-15

---

## O que a IA fez

A IA preparou todo o código e configuração necessários para o deploy no Lambda. O repositório está pronto — as etapas abaixo são intervenções manuais na AWS e no GitHub que requerem acesso humano.

---

## Intervenções necessárias

### 1. Rotacionar credenciais expostas no git

A connection string do banco (Neon.tech) e a chave JWT estavam commitadas em `appsettings.json` em versões anteriores do branch.

**Ações obrigatórias:**
- [ ] Rotacionar a senha do banco no painel do Neon.tech (ou no seu provider PostgreSQL).
- [ ] Gerar uma nova chave JWT (mínimo 32 caracteres aleatórios) para substituir a exposta.
- [ ] **Não** usar as credenciais antigas em nenhum ambiente — elas estão no histórico do git público.

---

### 2. Criar repositório no Amazon ECR

```bash
aws ecr create-repository \
  --repository-name edoha-api \
  --region <regiao>
```

Anote o URI retornado (`<conta>.dkr.ecr.<regiao>.amazonaws.com/edoha-api`).

---

### 3. Criar a função Lambda

Crie a função via console AWS ou CLI:

```bash
aws lambda create-function \
  --function-name edoha-api \
  --package-type Image \
  --code ImageUri=<conta>.dkr.ecr.<regiao>.amazonaws.com/edoha-api:latest \
  --role arn:aws:iam::<conta>:role/<nome-da-role> \
  --memory-size 512 \
  --timeout 30 \
  --region <regiao>
```

**Pré-requisito:** a IAM Role precisa ter as policies:
- `AWSLambdaBasicExecutionRole` (CloudWatch Logs)
- `AmazonEC2ContainerRegistryReadOnly` (ler imagem do ECR)
- Se banco em VPC: `AWSLambdaVPCAccessExecutionRole`

---

### 4. Configurar variáveis de ambiente na função Lambda

No console AWS (Lambda → Configuration → Environment variables) ou via CLI:

```bash
aws lambda update-function-configuration \
  --function-name edoha-api \
  --environment "Variables={
    ASPNETCORE_ENVIRONMENT=Production,
    Jwt__Issuer=EdohaIssuer,
    Jwt__Audience=EdohaAudience,
    Jwt__ExpiresInMinutes=60,
    Cors__AllowedOrigins__0=https://<seu-dominio-frontend>
  }" \
  --region <regiao>
```

Para `ConnectionStrings__Default` e `Jwt__Key`, **use AWS Secrets Manager**:

```bash
# Criar segredo da connection string
aws secretsmanager create-secret \
  --name /edoha/prod/connection-string \
  --secret-string "Host=...;Port=5432;Database=edoha;Username=...;Password=<nova-senha>;SslMode=Require" \
  --region <regiao>

# Criar segredo da chave JWT
aws secretsmanager create-secret \
  --name /edoha/prod/jwt-key \
  --secret-string "<nova-chave-jwt-min-32-chars>" \
  --region <regiao>
```

Em seguida, configure as env vars da função para ler do Secrets Manager via extensão, ou injete os valores diretamente como variáveis de ambiente criptografadas pelo Lambda (mais simples para começar).

---

### 5. Criar API Gateway HTTP API

Via SAM (recomendado):

```bash
cd deploy
sam build --template template.yaml
sam deploy --guided
```

O `--guided` vai criar o `samconfig.toml` para deploys futuros. Preencha os placeholders do `template.yaml` antes de rodar.

Ou crie manualmente via console e aponte para a função `edoha-api`.

---

### 6. Configurar variáveis e secrets no repositório GitHub

Acesse o repositório → Settings → Secrets and variables → Actions.

**Secrets:**
| Nome | Valor |
|---|---|
| `AWS_ROLE_TO_ASSUME` | ARN da IAM Role do GitHub OIDC (ver abaixo) |

**Variables:**
| Nome | Valor |
|---|---|
| `AWS_REGION` | Ex: `sa-east-1` |
| `ECR_REPOSITORY` | `edoha-api` |
| `LAMBDA_FUNCTION_NAME` | `edoha-api` |

---

### 7. Criar IAM Role para o GitHub Actions (OIDC)

Para autenticação sem credenciais de longa duração:

1. No console IAM, criar um **Identity Provider** do tipo OpenID Connect:
   - Provider URL: `https://token.actions.githubusercontent.com`
   - Audience: `sts.amazonaws.com`

2. Criar uma Role com trust policy:
```json
{
  "Version": "2012-10-17",
  "Statement": [{
    "Effect": "Allow",
    "Principal": { "Federated": "arn:aws:iam::<conta>:oidc-provider/token.actions.githubusercontent.com" },
    "Action": "sts:AssumeRoleWithWebIdentity",
    "Condition": {
      "StringEquals": { "token.actions.githubusercontent.com:aud": "sts.amazonaws.com" },
      "StringLike": { "token.actions.githubusercontent.com:sub": "repo:<org>/<repo>:*" }
    }
  }]
}
```

3. Adicionar as policies necessárias à Role:
   - `AmazonEC2ContainerRegistryPowerUser` (push ECR)
   - Permissão inline para `lambda:UpdateFunctionCode` na função `edoha-api`

---

### 8. (Opcional) Configurar VPC

Se o banco PostgreSQL estiver em VPC privada:
- Descomentar o bloco `VpcConfig` em `deploy/template.yaml`.
- Preencher os IDs de subnets e security group.
- Garantir que o security group da função Lambda tem permissão de saída para a porta 5432 do RDS.

---

### 9. Verificar healthcheck após primeiro deploy

```bash
curl https://<api-gateway-url>/health
# Esperado: {"status":"Healthy"}
```
