using Edoha.Domain.Entities;
using Edoha.Domain.Models.DTOs.Permission;
using Edoha.Domain.Interfaces.Infraestructure.Context;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Interfaces.Domain.Services;

namespace Edoha.Domain.Services
{
    public class PermissionService : Service<Permission>, IPermissionService
    {
        private readonly IInstitutionRepository _institutionRepository;
        public PermissionService(IInstitutionRepository institutionRepository, IPermissionRepository repository,
            IRequestValidationContext requestValidationContext)
            : base(repository, requestValidationContext) 
        {
            _institutionRepository = institutionRepository;
        }

        public async Task InsertPermission(CreatePermissionDTO dto)
        {
            await _institutionRepository.IdExists(dto.IdInstitution);
            await Insert(dto);
        }

        public async Task<Permission?> SelectPermissionById(Guid id)
        {
            return await _repository.SelectById(id);
        }

        public async Task<IEnumerable<Permission>> SelectAllPermissions()
        {
            return await _repository.SelectAll();
        }

        public async Task UpdatePermissionById(UpdatePermissionDTO dto)
        {
            await Update(dto);
        }

        public async Task DeletePermissionById(Guid id)
        {
            await DeleteById(id);
        }
    }
}
