using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Domain.Services;
using Edoha.Domain.Interfaces.Infraestructure.Context;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Models.DTOs.Institution;
using Microsoft.Extensions.Logging;

namespace Edoha.Domain.Services
{
    public class InstitutionService : Service<Institution>, IInstitutionService
    {
        private readonly IInstitutionRepository _institutionRepository;
        private readonly ILogger<InstitutionService> _logger;

        public InstitutionService(IInstitutionRepository repository, 
            IRequestValidationContext requestValidationContext,
            ILogger<InstitutionService> logger
            ) : base(repository, requestValidationContext, logger)
        {
            _institutionRepository = repository;
            _logger = logger;
        }

        public async Task InsertInstitution(CreateInstitutionDTO dto)
        {
            _logger.LogInformation("Iniciando método InsertInstitution (InstitutionService)");
            _logger.LogInformation("Parâmetros recebidos - dto: {@Dto}", dto);
            
            _logger.LogInformation("Chamando método Insert da classe base Service");
            await Insert(dto);
            
            _logger.LogInformation("Método InsertInstitution finalizado com sucesso");
        }

        public async Task<Institution?> SelectInstitutionBySlug(string slug)
        {
            _logger.LogInformation("Iniciando método SelectInstitutionBySlug (InstitutionService)");
            _logger.LogInformation("Parâmetros recebidos - slug: {Slug}", slug);
            
            _logger.LogInformation("Chamando _institutionRepository.SelectInstitutionBySlug");
            var institution = await _institutionRepository.SelectInstitutionBySlug(slug);
            
            if (institution == null)
            {
                _logger.LogInformation("Instituição com slug {Slug} não encontrada. Retornando null", slug);
            }
            else
            {
                _logger.LogInformation("Instituição com slug {Slug} encontrada. Retornando resultado", slug);
            }
            
            return institution;
        }

        public async Task<IEnumerable<Institution>> SelectAllInstitutions()
        {
            _logger.LogInformation("Iniciando método SelectAllInstitutions (InstitutionService)");
            
            _logger.LogInformation("Chamando _repository.SelectAll");
            var institutions = await _repository.SelectAll();
            
            _logger.LogInformation("Método SelectAllInstitutions finalizado. Retornando {Count} instituições", institutions.Count());
            return institutions;
        }

        public async Task<IEnumerable<Institution>> SelectInstitutionsByUser(Guid idUser)
        {
            _logger.LogInformation("Iniciando método SelectInstitutionsByUser (InstitutionService)");
            _logger.LogInformation("Parâmetros recebidos - idUser: {IdUser}", idUser);
            
            _logger.LogInformation("Chamando _institutionRepository.SelectInstitutionsByUser");
            var institutions = await _institutionRepository.SelectInstitutionsByUser(idUser);
            
            _logger.LogInformation("Método SelectInstitutionsByUser finalizado. Retornando {Count} instituições para o usuário {IdUser}", institutions.Count(), idUser);
            return institutions;
        }

        public async Task UpdateInstitutionById(UpdateInstitutionDTO dto)
        {
            _logger.LogInformation("Iniciando método UpdateInstitutionById (InstitutionService)");
            _logger.LogInformation("Parâmetros recebidos - dto: {@Dto}", dto);
            
            _logger.LogInformation("Chamando método Update da classe base Service");
            await Update(dto);
            
            _logger.LogInformation("Método UpdateInstitutionById finalizado com sucesso");
        }

        public async Task DeleteInstitutionById(Guid id)
        {
            _logger.LogInformation("Iniciando método DeleteInstitutionById (InstitutionService)");
            _logger.LogInformation("Parâmetros recebidos - id: {Id}", id);
            
            _logger.LogInformation("Chamando método DeleteById da classe base Service");
            await DeleteById(id);
            
            _logger.LogInformation("Método DeleteInstitutionById finalizado com sucesso para id {Id}", id);
        }
    }
}
