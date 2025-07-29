using Edoha.Domain.Models.DTOs;
using Edoha.Domain.Interfaces.Infraestructure.Context;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Interfaces.Domain.Services;

namespace Edoha.Domain.Services
{
    public abstract class Service<T> : IService<T> where T : class
    {
        protected readonly IBaseRepository<T> _repository;
        protected readonly IRequestValidationContext _requestValidationContext;

        public Service(IBaseRepository<T> repository, IRequestValidationContext requestValidationContext)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _requestValidationContext = requestValidationContext;
        }

        public async Task Insert(DTO dto)
        {
            await _requestValidationContext.ValidateDTO(dto);
            await _repository.Insert(dto);
        }

        public async Task Update(DTO dto)
        {
            await _requestValidationContext.ValidateDTO(dto);
            await _repository.Update(dto);
        }

        public async Task DeleteById(Guid id)
        {
            await _repository.IdExists(id);
            await _repository.DeleteById(id);
        }
    }
}
