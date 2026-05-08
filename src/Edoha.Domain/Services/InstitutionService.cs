using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Domain.Services;
using Edoha.Domain.Interfaces.Infraestructure.Context;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Models.DTOs.Institution;

namespace Edoha.Domain.Services
{
    public class InstitutionService : Service<Institution>, IInstitutionService
    {
        private readonly IInstitutionRepository _institutionRepository;

        public InstitutionService(IInstitutionRepository repository, 
            IRequestValidationContext requestValidationContext
            ) : base(repository, requestValidationContext)
        {
            _institutionRepository = repository;
        }

        public async Task InsertInstitution(CreateInstitutionDTO dto)
        {
            await Insert(dto);
        }

        public async Task<Institution?> SelectInstitutionBySlug(string slug)
        {
            return await _institutionRepository.SelectInstitutionBySlug(slug);
        }

        public async Task<IEnumerable<Institution>> SelectAllInstitutions()
        {
            return await _repository.SelectAll();
        }

        public async Task<IEnumerable<Institution>> SelectInstitutionsByUser(Guid idUser)
        {
            return await _institutionRepository.SelectInstitutionsByUser(idUser);
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
