using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Domain.Services;
using Edoha.Domain.Interfaces.Infraestructure.Context;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Models.DTOs.Institution;

namespace Edoha.Domain.Services
{
    public class InstitutionService : Service<Institution>, IInstitutionService
    {

        public InstitutionService(IInstitutionRepository repository, 
            IRequestValidationContext requestValidationContext
            ) : base(repository, requestValidationContext) { }

        public async Task InsertInstitution(CreateInstitutionDTO dto)
        {
            await Insert(dto);
        }

        public async Task<Institution> SelectInstitutionById(Guid id)
        {
            return await _repository.SelectById(id);
        }

        public async Task<IEnumerable<Institution>> SelectAllInstitutions()
        {
            return await _repository.SelectAll();
        }

        public async Task UpdateInstitutionById(UpdateInstitutionDTO dto)
        {
            await Update(dto);
        }

        public async Task DeleteInstitutionById(Guid id)
        {
            await DeleteById(id);
        }
    }
}
