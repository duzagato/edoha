using Dapper;
using Edoha.Domain.Entities;
using Edoha.Domain.Helpers;
using Edoha.Infrastructure.Repositories;
using Edoha.Infraestructure.Constants;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;

namespace Edoha.Infraestructure.Repositories
{
    public class InstitutionRepository : BaseRepository<Institution>, IInstitutionRepository
    {
        public InstitutionRepository(IDbConnection connection) : base(connection) 
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        public async Task<Institution?> SelectInstitutionBySlug(string slug)
        {
            string query = StaticQueries.SelectInstitutionBySlug;

            var institution = await _connection.QueryFirstOrDefaultAsync<Institution?>(
                query,
                new { Slug = slug }
            );

            return institution;
        }

        public async Task<IEnumerable<Institution>> SelectInstitutionsByUser(Guid idUser)
        {
            CheckConnection();

            return await _connection.QueryAsync<Institution>(StaticQueries.SelectInstitutionsByUser, new { IdUser = idUser });
        }
    }
}
