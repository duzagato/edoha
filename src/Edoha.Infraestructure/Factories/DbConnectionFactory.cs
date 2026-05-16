
using Edoha.Domain.Interfaces.Infraestructure.Factories;
using Edoha.Domain.Interfaces.Infraestructure.Services;
using Npgsql;
using System.Data;

namespace Edoha.Infraestructure.Factories
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly ISecretsManagerService _secretsManagerService;

        public DbConnectionFactory(ISecretsManagerService secretsManagerService)
        {
            _secretsManagerService = secretsManagerService;
        }

        public IDbConnection CreateConnection()
        {
            var connectionString = _secretsManagerService.GetConnectionStringAsync().Result;
            var connection = new NpgsqlConnection(connectionString);
            
            ((NpgsqlConnection)
            connection).Open();

            return connection;
        }
    }
}
