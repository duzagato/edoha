using System.Data;
using Edoha.Domain.Interfaces.Infraestructure.Context;
using Npgsql;

namespace Edoha.Infraestructure.Context
{
    public class DbConnectionContext : IDbConnectionContext
    {
        private readonly string _connectionString;

        public DbConnectionContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IDbConnection CreateConnection()
        {
            var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            return connection;
        }
    }
}