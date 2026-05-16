namespace Edoha.Infraestructure.Constants
{
    /// <summary>
    /// Centraliza as chaves (nomes/ARNs) dos segredos armazenados no AWS Secrets Manager.
    /// Use estas constantes ao invocar <c>ISecretsManagerService.GetSecretAsync</c>.
    /// </summary>
    public static class SecretKeys
    {
        public const string DatabaseConnection = "ConnectionString";
        public const string JwtKey = "edoha/jwt/key";
    }
}
