namespace Edoha.Infraestructure.Constants
{
    /// <summary>
    /// Centraliza as chaves (nomes/ARNs) dos segredos armazenados no AWS Secrets Manager.
    /// Use estas constantes ao invocar <c>ISecretsManagerService.GetSecretAsync</c>.
    /// </summary>
    public static class SecretKeys
    {
        public const string PrivateKeys = "PrivateKeys";
        public const string DatabaseConnection = "connectionString";
        public const string JwtKey = "jwtKey";
        public const string MemoryCachePrefix = "Screts:";
    }
}
