using Dapper;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Infraestructure.Factories;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Infraestructure.Constants;
using Edoha.Infrastructure.Repositories;
using System.Data;

namespace Edoha.Infraestructure.Repositories
{
    public class LotteryRepository : BaseRepository<Lottery>, ILotteryRepository
    {
        public LotteryRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory) 
        {}

        public async Task<bool> LotteryIsUnique(Guid idInstitution, string name)
        {
            CheckConnection();

            var query = StaticQueries.LotteryIsUnique;

            var count = await _connection.ExecuteScalarAsync<int>(query, new { Id = idInstitution, Name = name });

            if (count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<IEnumerable<Lottery>> SelectAllByInstitution(Guid idInstitution)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            CheckConnection();

            var query = StaticQueries.SelectAllLotteriesByInstitution;

            return await _connection.QueryAsync<Lottery>(query, new { IdInstitution = idInstitution });
        }

    }
}
