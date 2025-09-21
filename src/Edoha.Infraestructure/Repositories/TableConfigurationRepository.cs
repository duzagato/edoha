using Dapper;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Infraestructure.Constants;
using Edoha.Infrastructure.Repositories;
using System.Data;

namespace Edoha.Infraestructure.Repositories
{
    public class TableConfigurationRepository : BaseRepository<TableConfiguration>, ITableConfigurationRepository
    {
        public TableConfigurationRepository(IDbConnection connection) : base(connection)
        {

        }

        public async Task<IEnumerable<TableConfiguration>?> SelectAllConfigurationsByTableName(string schema, string tableName)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            CheckConnection();

            string query = StaticQueries.SelectAllConfigurationsByTableName;

            var configurations = await _connection.QueryAsync<TableConfiguration>(query, new { Schema = schema, TableName = tableName });

            return configurations ?? null;
        }
    }
}