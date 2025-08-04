using Edoha.Domain.Entities;
using Edoha.Domain.Models.DTOs.Institution;

namespace Edoha.Domain.Interfaces.Domain.Services
{
    public interface IInstitutionService : IService<Institution>
    {
        public Task InsertInstitution(CreateInstitutionDTO dto);

        public Task<Institution> SelectInstitutionById(Guid id);

        public Task<IEnumerable<Institution>> SelectAllInstitutions();

        public Task UpdateInstitutionById(UpdateInstitutionDTO dto);

        public Task DeleteInstitutionById(Guid id);
    }
}
