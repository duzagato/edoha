namespace Edoha.Domain.Interfaces.Infraestructure.Services
{
    public interface ISecretsManagerService
    {
        /// <summary>
        /// Requisita o valor de um segredo armazenado no AWS Secrets Manager.
        /// </summary>
        /// <param name="secretKey">Chave/nome do segredo. Use as constantes em <c>SecretKeys</c>.</param>
        /// <returns>Valor do segredo (SecretString).</returns>
        Task<string> GetSecretAsync(string secretKey);
    }
}
