using Amazon.Runtime.Internal.Util;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Edoha.Domain.Interfaces.Infraestructure.Services;
using Edoha.Infraestructure.Constants;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace Edoha.Infraestructure.Services
{
    public class SecretsManagerService : ISecretsManagerService
    {
        private readonly IMemoryCache _cache;
        private readonly IAmazonSecretsManager _client;

        public SecretsManagerService(
            IMemoryCache cache,
            IAmazonSecretsManager client
        )
        {
            _cache = cache;
            _client = client;
        }

        public async Task<string> GetConnectionStringAsync()
        {
            var connectionStringKey = SecretKeys.DatabaseConnection;
            var secrets = await GetSecretDictionaryAsync(SecretKeys.PrivateKeys);
            var teste = secrets[connectionStringKey];

            return secrets[connectionStringKey];
        }

        public async Task<string> GetJwtKeyAsync()
        {
            var jwtSecretId = SecretKeys.JwtKey;
            var secrets = await GetSecretDictionaryAsync(SecretKeys.PrivateKeys);

            return secrets[jwtSecretId];
        }

        private async Task<string> GetSecretAsync(string secretKey)
        {
            var request = new GetSecretValueRequest
            {
                SecretId = secretKey
            };

            var response = await _client.GetSecretValueAsync(request);

            if (string.IsNullOrEmpty(response.SecretString))
            {
                throw new InvalidOperationException($"O segredo '{secretKey}' está vazio ou não é uma string válida.");
            }

            return response.SecretString;
        }

        private async Task<Dictionary<string, string>> GetSecretDictionaryAsync(string secretId)
        {
            string cacheKey = SecretKeys.MemoryCachePrefix + secretId;

            if (_cache.TryGetValue(cacheKey, out Dictionary<string, string> cachedSecret))
            {
                return cachedSecret;
            }

            var secretString = await GetSecretAsync(secretId);
            var secretDictionary = JsonSerializer.Deserialize<Dictionary<string, string>>(secretString);
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(30));
            _cache.Set(cacheKey, secretDictionary, cacheEntryOptions);

            return secretDictionary!;
        }
    }
}
