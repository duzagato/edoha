using Dapper;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Infraestructure.Constants;
using Edoha.Infrastructure.Repositories;
using System.Data;
using Action = Edoha.Domain.Entities.Action;

namespace Edoha.Infraestructure.Repositories
{
    public class ActionRepository : BaseRepository<Action>, IActionRepository
    {
        public ActionRepository(IDbConnection connection) : base(connection)
        {

        }

        public async Task<IEnumerable<Action>> SelectUserActions(Guid idUser, string pageName)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            CheckConnection();

            string query = StaticQueries.SelectUserActions;

            var actions = await _connection.QueryAsync<Action>(query, new { IdUser = idUser, PageName = pageName });

            return actions ?? throw new KeyNotFoundException("Entidade não encontrada");
        }
    }
}
