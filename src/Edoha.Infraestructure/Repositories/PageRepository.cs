using Dapper;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Infraestructure.Constants;
using Edoha.Infrastructure.Repositories;
using System.Data;

namespace Edoha.Infraestructure.Repositories
{
    public class PageRepository : BaseRepository<Page>, IPageRepository
    {
        public PageRepository(IDbConnection connection) : base(connection)
        {

        }

        public async Task<IEnumerable<string>> SelectPagesPermissionsByIdUser(Guid idUser)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            CheckConnection();

            string query = StaticQueries.SelectPagesPermissionsByIdUser;

            var pages = await _connection.QueryAsync<string>(query, new { IdUser = idUser });

            return pages ?? throw new KeyNotFoundException("Entidade não encontrada");
        }
    }
}
