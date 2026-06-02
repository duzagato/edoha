using Dapper;
using Edoha.Domain.Entities;
using Edoha.Domain.Helpers;
using Edoha.Domain.Interfaces.Infraestructure.Factories;
using Edoha.Infrastructure.Repositories;
using Edoha.Infraestructure.Constants;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Microsoft.Extensions.Logging;

namespace Edoha.Infraestructure.Repositories
{
    public class InstitutionRepository : BaseRepository<Institution>, IInstitutionRepository
    {
        private readonly ILogger<InstitutionRepository> _repositoryLogger;
        
        public InstitutionRepository(IDbConnectionFactory connectionFactory, ILogger<InstitutionRepository> logger) 
            : base(connectionFactory, logger) 
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            _repositoryLogger = logger;
        }

        public async Task<Institution?> SelectInstitutionBySlug(string slug)
        {
            _repositoryLogger.LogInformation("Iniciando método SelectInstitutionBySlug (InstitutionRepository)");
            _repositoryLogger.LogInformation("Parâmetros recebidos - slug: {Slug}", slug);
            
            string query = StaticQueries.SelectInstitutionBySlug;

            _repositoryLogger.LogInformation("Executando query para selecionar instituição por slug");
            var institution = await _connection.QueryFirstOrDefaultAsync<Institution?>(
                query,
                new { Slug = slug }
            );

            if (institution == null)
            {
                _repositoryLogger.LogInformation("Instituição com slug {Slug} não encontrada. Retornando null", slug);
            }
            else
            {
                _repositoryLogger.LogInformation("Instituição com slug {Slug} encontrada. Retornando resultado", slug);
            }

            return institution;
        }

        public async Task<IEnumerable<Institution>> SelectInstitutionsByUser(Guid idUser)
        {
            _repositoryLogger.LogInformation("Iniciando método SelectInstitutionsByUser (InstitutionRepository)");
            _repositoryLogger.LogInformation("Parâmetros recebidos - idUser: {IdUser}", idUser);
            
            CheckConnection();

            _repositoryLogger.LogInformation("Executando query para selecionar instituições por usuário");
            var institutions = await _connection.QueryAsync<Institution>(StaticQueries.SelectInstitutionsByUser, new { IdUser = idUser });
            
            _repositoryLogger.LogInformation("Método SelectInstitutionsByUser finalizado. Retornando {Count} instituições para o usuário {IdUser}", institutions.Count(), idUser);
            return institutions;
        }
    }
}
