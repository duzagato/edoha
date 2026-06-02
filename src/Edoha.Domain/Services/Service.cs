using Edoha.Domain.Models.DTOs;
using Edoha.Domain.Interfaces.Infraestructure.Context;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Interfaces.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Edoha.Domain.Services
{
    public abstract class Service<T> : IService<T> where T : class
    {
        protected readonly IBaseRepository<T> _repository;
        protected readonly IRequestValidationContext _requestValidationContext;
        protected readonly ILogger _logger;

        public Service(IBaseRepository<T> repository, IRequestValidationContext requestValidationContext, ILogger logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _requestValidationContext = requestValidationContext;
            _logger = logger;
        }

        // Constructor for backward compatibility with services that don't have logger yet
        public Service(IBaseRepository<T> repository, IRequestValidationContext requestValidationContext)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _requestValidationContext = requestValidationContext;
            _logger = null!; // Will be null for services not yet updated
        }

        public async Task Insert(DTO dto)
        {
            _logger?.LogInformation("Iniciando método Insert (Service base)");
            _logger?.LogInformation("Parâmetros recebidos - dto: {@Dto}", dto);

            _logger?.LogInformation("Validando DTO");
            await _requestValidationContext.ValidateDTO(dto);

            _logger?.LogInformation("Inserindo entidade no repositório");
            await _repository.Insert(dto);

            _logger?.LogInformation("Entidade inserida com sucesso");
        }

        public async Task Update(DTO dto)
        {
            _logger?.LogInformation("Iniciando método Update (Service base)");
            _logger?.LogInformation("Parâmetros recebidos - dto: {@Dto}", dto);

            _logger?.LogInformation("Validando DTO");
            await _requestValidationContext.ValidateDTO(dto);

            _logger?.LogInformation("Atualizando entidade no repositório");
            await _repository.Update(dto);

            _logger?.LogInformation("Entidade atualizada com sucesso");
        }

        public async Task DeleteById(Guid id)
        {
            _logger?.LogInformation("Iniciando método DeleteById (Service base)");
            _logger?.LogInformation("Parâmetros recebidos - id: {Id}", id);

            _logger?.LogInformation("Verificando se entidade com id {Id} existe", id);
            await _repository.IdExists(id);

            _logger?.LogInformation("Deletando entidade com id {Id}", id);
            await _repository.DeleteById(id);

            _logger?.LogInformation("Entidade com id {Id} deletada com sucesso", id);
        }
    }
}
