using Edoha.Domain.Entities;
using Edoha.Domain.Models.DTOs.UserPermission;
using Edoha.Domain.Interfaces.Infraestructure.Context;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Interfaces.Domain.Services;

namespace Edoha.Domain.Services
{
    public class UserPermissionService : Service<UserPermission>, IUserPermissionService
    {
        private readonly IActionRepository _actionRepository;
        private readonly IPageRepository _pageRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUserRepository _userRepository;

        public UserPermissionService(
            IActionRepository actionRepository,
            IPageRepository pageRepository,
            IPermissionRepository permissionRepository,
            IUserRepository userRepository,
            IUserPermissionRepository repository,
            IRequestValidationContext requestValidationContext
        ) : base(repository, requestValidationContext)
        {
            _actionRepository = actionRepository;
            _pageRepository = pageRepository;
            _permissionRepository = permissionRepository;
            _userRepository = userRepository;
        }

        public async Task InsertUserPermission(CreateUserPermissionDTO dto)
        {
            await _actionRepository.IdExists(dto.IdAction);
            await _pageRepository.IdExists(dto.IdPage);
            await _permissionRepository.IdExists(dto.IdPermission);
            await _userRepository.IdExists(dto.IdUser);
            await Insert(dto);
        }

        public async Task<UserPermission?> SelectUserPermissionById(Guid id)
        {
            return await _repository.SelectById(id);
        }

        public async Task<IEnumerable<UserPermission>> SelectAllUserPermissions()
        {
            return await _repository.SelectAll();
        }

        public async Task DeleteUserPermissionById(Guid id)
        {
            await DeleteById(id);
        }
    }
}
