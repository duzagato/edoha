using Dapper;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Infraestructure.Factories;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Infraestructure.Constants;
using Edoha.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Edoha.Infraestructure.Repositories
{
    public class LotteryRepository : BaseRepository<Lottery>, ILotteryRepository
    {
        private readonly ILogger<LotteryRepository> _logger;

        public LotteryRepository(IDbConnectionFactory connectionFactory, ILogger<LotteryRepository> logger)
            : base(connectionFactory)
        {
            _logger = logger;
        }

        public async Task<bool> LotteryIsUnique(Guid idInstitution, string name)
        {
            _logger.LogInformation("[LotteryRepository.LotteryIsUnique] Iniciado. Params: idInstitution={idInstitution}, name={name}", idInstitution, name);

            CheckConnection();

            var query = StaticQueries.LotteryIsUnique;

            _logger.LogInformation("[LotteryRepository.LotteryIsUnique] Executando query de unicidade.");
            var count = await _connection.ExecuteScalarAsync<int>(query, new { Id = idInstitution, Name = name });

            bool isUnique = count == 0;
            _logger.LogInformation("[LotteryRepository.LotteryIsUnique] Contagem={count}, IsUnique={isUnique}. Retornando {isUnique}.", count, isUnique, isUnique);

            return isUnique;
        }

        public async Task<IEnumerable<Lottery>> SelectAllByInstitution(Guid idInstitution)
        {
            _logger.LogInformation("[LotteryRepository.SelectAllByInstitution] Iniciado. Params: idInstitution={idInstitution}", idInstitution);

            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            CheckConnection();

            var query = StaticQueries.SelectAllLotteriesByInstitution;

            _logger.LogInformation("[LotteryRepository.SelectAllByInstitution] Executando query para a instituição {idInstitution}.", idInstitution);
            var lotteries = await _connection.QueryAsync<Lottery>(query, new { IdInstitution = idInstitution });

            _logger.LogInformation("[LotteryRepository.SelectAllByInstitution] Retornando {count} rifa(s) para a instituição {idInstitution}.", lotteries.Count(), idInstitution);
            return lotteries;
        }
    }
}
