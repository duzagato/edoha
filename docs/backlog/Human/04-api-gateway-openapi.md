# 04 — Criação da API no AWS API Gateway via OpenAPI

## Parte 1 — O que complementar no arquivo `openapi.yaml`

### 1.1 — URL do servidor

O campo `servers.url` está com um placeholder:

```yaml
servers:
  - url: https://{baseUrl}
    variables:
      baseUrl:
        default: localhost
```

Após criar a API no API Gateway, substitua pelo endpoint gerado:

```yaml
servers:
  - url: https://<api-id>.execute-api.<região>.amazonaws.com/<stage>
```

---

### 1.2 — Schemas de entidades com propriedades mínimas

Os schemas abaixo foram definidos com propriedades básicas inferidas pelo código, mas podem estar incompletos dependendo do que a query retorna no banco. Confirme os campos reais e adicione o que estiver faltando:

| Schema | Campos definidos | O que verificar |
|---|---|---|
| `Institution` | `id`, `name`, `slug` | Há mais campos retornados pelo SELECT? |
| `UserInstitution` | `id`, `idUser`, `idInstitution` | Idem |
| `User` | `id`, `name`, `phone`, `nickname` | `idUserType`, `active`, etc.? |
| `Ticketbook` | Campos principais | Campos calculados ou joinados retornados? |
| `Ticket` | Campos principais | Há campos de `donaterName`/`donaterPhone` retornados no GET? |

---

### 1.3 — Endpoints com requestBody ausente (DTOs não implementados)

Os endpoints abaixo existem no código mas os DTOs estão vazios, então o `openapi.yaml` não tem `requestBody` definido para eles. Quando os DTOs forem implementados, adicione o schema correspondente:

| Endpoint | DTO a implementar |
|---|---|
| `POST /institution` | `CreateInstitutionDTO` |
| `PUT /institution` | `UpdateInstitutionDTO` |
| `POST /userinstitution` | `CreateUserInstitutionDTO` |
| `PUT /userinstitution` | `UpdateUserInstitutionDTO` |
| `PUT /user` | `UpdateUserDTO` |
| `PUT /statusticketbook` | `UpdateStatusTicketbookDTO` |

---

### 1.4 — CORS (se o frontend acessar a API diretamente)

Se um frontend web precisar chamar a API diretamente, adicione o suporte a CORS no API Gateway (veja Parte 2, passo 6). O YAML atual não inclui métodos `OPTIONS` — o API Gateway pode gerá-los automaticamente ao ativar CORS no console.

---

### 1.5 — Autenticação JWT (quando estiver pronta)

O arquivo atual não tem segurança configurada. Quando o JWT estiver operacional, adicione ao YAML:

```yaml
# Em components:
securitySchemes:
  bearerAuth:
    type: http
    scheme: bearer
    bearerFormat: JWT

# Global (aplica a todos os endpoints):
security:
  - bearerAuth: []

# Em endpoints públicos (ex: POST /auth), sobrescreva com:
security: []
```

No API Gateway, configure um **Lambda Authorizer** ou um **Cognito Authorizer** apontando para o mesmo mecanismo JWT da aplicação.

---

## Parte 2 — Passo a passo: criar a API no AWS API Gateway

> **Contexto:** a API é uma aplicação ASP.NET Core rodando dentro de uma Lambda (via `Amazon.Lambda.AspNetCoreServer`). O API Gateway deve atuar como proxy, repassando cada requisição à Lambda e devolvendo a resposta ao cliente.

---

### Passo 1 — Abrir o API Gateway no console AWS

1. Acesse o [Console AWS](https://console.aws.amazon.com).
2. Navegue em **Services → API Gateway**.
3. Clique em **Create API**.
4. Escolha **REST API** (não "HTTP API" nem "WebSocket") → clique em **Build**.

> Use **REST API** (não a versão privada) para ter suporte completo a importação OpenAPI, mapeamento de responses e stages.

---

### Passo 2 — Importar o arquivo OpenAPI

1. Em **Create new API**, selecione **Import from OpenAPI 3**.
2. Clique em **Select file** e envie o arquivo `openapi.yaml` da raiz do repositório.
3. Em **Endpoint Type**, selecione **Regional**.
4. Clique em **Import**.

O API Gateway criará automaticamente todos os recursos (paths) e métodos (GET, POST, PUT, etc.) definidos no YAML.

> Se aparecerem avisos de rotas ambíguas (ex: `/statusticketbook/{id}` vs rotas estáticas no Ticketbook), são esperados — o API Gateway resolve por ordem de prioridade de rotas estáticas.

---

### Passo 3 — Configurar a integração com a Lambda

Após a importação, cada método foi criado **sem integração**. É necessário vincular cada um à Lambda:

**Opção A — Configurar método a método (mais controle):**

1. No painel de recursos, clique em um método (ex: `GET /auth`).
2. Clique em **Integration Request**.
3. Em **Integration type**, selecione **Lambda Function**.
4. Marque **Use Lambda Proxy integration**.
5. Em **Lambda Function**, informe o ARN da Lambda (ou o nome da função se estiver na mesma região).
6. Clique em **Save** → confirme a permissão de invocação quando solicitado.
7. Repita para cada método.

**Opção B — Usar um recurso proxy catch-all `/{proxy+}` (mais rápido):**

Se todos os métodos apontam para a mesma Lambda, você pode apagar os recursos importados e criar um único recurso proxy:

1. Clique em **Actions → Create Resource**.
2. Marque **Configure as proxy resource**.
3. Nome do recurso: `{proxy}`, path: `/{proxy+}`.
4. Marque **Enable API Gateway CORS**.
5. Em **Lambda Function Proxy**, informe o ARN da Lambda.
6. Clique em **Save**.

> A Opção B é mais simples e funciona bem com ASP.NET Core no Lambda, já que o `Amazon.Lambda.AspNetCoreServer` roteará internamente. Use a Opção A se quiser validação de request/response no nível do API Gateway.

---

### Passo 4 — Verificar permissão da Lambda

O API Gateway precisa de permissão para invocar a Lambda. Se não foi concedida automaticamente no passo anterior, execute no terminal:

```bash
aws lambda add-permission \
  --function-name <nome-da-funcao> \
  --statement-id apigateway-invoke \
  --action lambda:InvokeFunction \
  --principal apigateway.amazonaws.com \
  --source-arn "arn:aws:execute-api:<regiao>:<account-id>:<api-id>/*"
```

---

### Passo 5 — Fazer o deploy para um stage

1. Clique em **Actions → Deploy API**.
2. Em **Deployment stage**, selecione **[New Stage]**.
3. Informe o nome do stage (ex: `dev`, `hml`, `prod`).
4. Clique em **Deploy**.

Ao final, o console exibirá o **Invoke URL** no formato:

```
https://<api-id>.execute-api.<região>.amazonaws.com/<stage>
```

Copie essa URL e atualize o campo `servers.url` no `openapi.yaml` (ver item 1.1).

---

### Passo 6 — Habilitar CORS (se necessário)

1. Selecione o recurso raiz (`/`) no painel de recursos.
2. Clique em **Actions → Enable CORS**.
3. Informe as origens permitidas (ex: `https://meusite.com` ou `*` para testes).
4. Clique em **Enable CORS and replace existing CORS headers**.
5. Faça um novo deploy (passo 5) para aplicar.

---

### Passo 7 — Testar a API

Use o **Test** embutido no console ou um cliente como o Insomnia/Postman:

1. No console, selecione um método → clique em **Test**.
2. Preencha path parameters e body conforme o `openapi.yaml`.
3. Verifique o status e o response body retornados pela Lambda.

Para testar via curl:

```bash
# Exemplo: autenticar
curl -X POST https://<invoke-url>/auth \
  -H "Content-Type: application/json" \
  -d '{"nickname": "john.doe", "password": "secret123"}'
```

---

### Observações finais

- **Throttling:** configure em **Usage Plans** para proteger a Lambda de picos de requisição.
- **Logs:** ative **CloudWatch Logs** no stage para depurar erros de integração.
- **Variáveis de ambiente da Lambda:** verifique se `ConnectionStrings__Default` e as configurações do Secrets Manager estão definidas na Lambda (ver `02-aws-secrets-manager.md`).
