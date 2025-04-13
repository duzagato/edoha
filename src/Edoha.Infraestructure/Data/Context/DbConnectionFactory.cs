using System.Data;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace Edoha.Infrastructure.Data.Context;

public class DbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
    {
        var connection = new SqlConnection(_connectionString);
        connection.Open(); // Abre a conexão automaticamente
        return connection;
    }
}
