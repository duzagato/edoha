# Estrutura para AWS Secrets Manager

**Data:** 2026-05-16
**Branch:** copilot/add-aws-secrets-manager-structure

## Resumo

Adicionada estrutura para consumir segredos do **AWS Secrets Manager**, seguindo o padrão atual do projeto (interface em `Edoha.Domain`, implementação em `Edoha.Infraestructure`, registro via DI em `Edoha.Application/DependencyInjection`).

A solicitação foi:
1. Centralizar as chaves dos segredos num arquivo de constantes dentro de `Constants` da camada de infra.
2. Padrão direto, alinhado ao design pattern atual.
3. Método obrigatório: enviar uma chave de segredo, fazer a requisição e retornar o valor.

## Arquivos criados

### `src/Edoha.Infraestructure/Constants/SecretKeys.cs`
Classe estática centralizando os nomes/ARNs dos segredos no AWS Secrets Manager. Hoje contém placeholders (`DatabaseConnection`, `JwtKey`) — adicione novas constantes aqui sempre que cadastrar um novo segredo no AWS.

### `src/Edoha.Domain/Interfaces/Infraestructure/Services/ISecretsManagerService.cs`
Interface do serviço. Expõe `Task<string> GetSecretAsync(string secretKey)` — o método obrigatório que recebe a chave e retorna o valor do segredo.

### `src/Edoha.Infraestructure/Services/SecretsManagerService.cs`
Implementação usando `IAmazonSecretsManager` (AWS SDK). Recebe a chave do segredo, monta um `GetSecretValueRequest` e retorna `SecretString`. Valida entrada nula/vazia lançando `ArgumentException`.

## Arquivos modificados

### `src/Edoha.Infraestructure/Edoha.Infraestructure.csproj`
- Adicionado pacote `AWSSDK.SecretsManager 4.0.4.20`.

### `src/Edoha.Application/DependencyInjection/ServiceInjection.cs`
- Registrado `IAmazonSecretsManager` como **Singleton** (cliente AWS é thread-safe e a recomendação oficial é reusar a instância).
- Registrado `ISecretsManagerService` → `SecretsManagerService` como **Scoped** (seguindo o padrão dos demais services).

## Como usar

```csharp
public class ExemploService
{
    private readonly ISecretsManagerService _secrets;

    public ExemploService(ISecretsManagerService secrets) => _secrets = secrets;

    public async Task FazerAlgo()
    {
        var connString = await _secrets.GetSecretAsync(SecretKeys.DatabaseConnection);
        // usar connString...
    }
}
```

## Observações sobre região e credenciais AWS

O `AmazonSecretsManagerClient` é instanciado sem parâmetros — ele resolve região e credenciais a partir do ambiente padrão da AWS SDK, na seguinte ordem:

1. Variáveis de ambiente (`AWS_REGION`, `AWS_ACCESS_KEY_ID`, `AWS_SECRET_ACCESS_KEY`).
2. Arquivo `~/.aws/credentials` / `~/.aws/config`.
3. **IAM Role** anexada ao recurso (Lambda Execution Role, EC2 Instance Profile, ECS Task Role).

No deploy via Lambda (que já está configurado neste projeto) a forma recomendada é conceder a permissão `secretsmanager:GetSecretValue` à Execution Role da Lambda — não há necessidade de injetar credenciais.
