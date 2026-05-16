using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Edoha.Domain.Interfaces.Infraestructure.Services;
using Edoha.Infraestructure.Constants;

namespace Edoha.Infraestructure.Services
{
    public class SecretsManagerService : ISecretsManagerService
    {
        private readonly IAmazonSecretsManager _client;

        public SecretsManagerService(IAmazonSecretsManager client)
        {
            _client = client;
        }

        public async Task<string> GetConnectionStringAsync()
        {
            var connectionStringKey = SecretKeys.DatabaseConnection;

            return await GetSecretAsync(connectionStringKey);
        }

        private async Task<string> GetSecretAsync(string secretKey)
        {
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new ArgumentException("A chave do segredo não pode ser vazia.", nameof(secretKey));

            var request = new GetSecretValueRequest
            {
                SecretId = secretKey
            };

            var response = await _client.GetSecretValueAsync(request);

            return response.SecretString;
        }
    }
}
