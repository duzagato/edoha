using Dapper;
using Edoha.Domain.Entities;
using Edoha.Infrastructure.Repositories;
using System.Data;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;

namespace Edoha.Infraestructure.Repositories
{
    public class StatusTicketbookRepository : BaseRepository<StatusTicketbook>, IStatusTicketbookRepository
    {
        public StatusTicketbookRepository(IDbConnection connection) : base(connection) 
        { 
            
        }

        public async Task<StatusTicketbook?> SelectByCode(int code)
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            CheckConnection();

            var query = $@"SELECT * FROM ""{_schema}"".""{_tableName}"" WHERE ""{_idColumnSnakeCase}"" = @Id";

            return await _connection.QueryFirstOrDefaultAsync<StatusTicketbook>(query, new { Id = code });
        }
    }
}
