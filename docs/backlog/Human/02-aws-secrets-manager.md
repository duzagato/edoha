# Intervenções humanas — AWS Secrets Manager

A estrutura de código foi entregue. Para que o `ISecretsManagerService` funcione em runtime, são necessárias as ações abaixo, que dependem de acesso ao console AWS / configuração de infraestrutura.

## 1. Cadastrar os segredos no AWS Secrets Manager

No console do AWS Secrets Manager (na região onde a aplicação roda), crie os segredos com os nomes definidos em `src/Edoha.Infraestructure/Constants/SecretKeys.cs`:

- `edoha/database/connection` — string de conexão completa do PostgreSQL.
- `edoha/jwt/key` — chave simétrica usada para assinar tokens JWT.

> Os nomes acima são placeholders. Ajuste tanto no console quanto no arquivo de constantes para refletir a sua convenção real, se for diferente.

## 2. Conceder permissão à Execution Role da Lambda

A função Lambda da API precisa ter a policy `secretsmanager:GetSecretValue` (no recurso ARN dos segredos criados). Exemplo de policy mínima:

```json
{
  "Effect": "Allow",
  "Action": ["secretsmanager:GetSecretValue"],
  "Resource": [
    "arn:aws:secretsmanager:<região>:<account-id>:secret:edoha/*"
  ]
}
```

Anexe essa policy à Execution Role configurada em `deploy/template.yaml`.

## 3. Configurar a região AWS

Garanta que a variável de ambiente `AWS_REGION` (ou `AWS_DEFAULT_REGION`) esteja definida no ambiente de execução (Lambda já injeta automaticamente; em ambiente local, configure no `appsettings.Development.json` indiretamente via variável de ambiente, ou em `~/.aws/config`).

## 4. (Opcional) Para rodar localmente

Configure credenciais AWS válidas:

```bash
aws configure
```

Ou exporte variáveis:

```bash
export AWS_REGION=us-east-1
export AWS_ACCESS_KEY_ID=...
export AWS_SECRET_ACCESS_KEY=...
```

## 5. Próximos passos sugeridos (não obrigatórios nesta task)

- Migrar a leitura da connection string e do `Jwt:Key` em `Program.cs` para usar o `ISecretsManagerService` no startup (substituindo o uso direto de `appsettings.json`).
- Adicionar cache em memória dos segredos para reduzir chamadas/custos do Secrets Manager.
