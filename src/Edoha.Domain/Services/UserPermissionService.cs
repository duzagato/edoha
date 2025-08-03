using Edoha.Domain.Entities;
using Action = Edoha.Domain.Entities.Action;
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
        private readonly IUserPermissionRepository _userPermissionRepository;

        public UserPermissionService(
            IActionRepository actionRepository,
            IPageRepository pageRepository,
            IPermissionRepository permissionRepository,
            IUserRepository userRepository,
            IUserPermissionRepository repository,
            IUserPermissionRepository userPermissionRepository,
            IRequestValidationContext requestValidationContext
        ) : base(repository, requestValidationContext)
        {
            _actionRepository = actionRepository;
            _pageRepository = pageRepository;
            _permissionRepository = permissionRepository;
            _userRepository = userRepository;
            _userPermissionRepository = userPermissionRepository;
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

        public async Task<List<UserPermissionPage>> GetUserPermissionsGroupByPageName(Guid idUser)
        {
            var userPermissions = await _userPermissionRepository.GetUserPermissionExpandByIdUser(idUser);

            var permissionsByPage = userPermissions
            .GroupBy(p => p.PageName)
            .Select(page => new UserPermissionPage(
                pageName: page.Key,
                actions: page.Select(p => new Action
                {
                    Name = p.ActionName,
                    WithoutOwner = p.WithoutOwner,
                    OtherOwner = p.OtherOwner
                }).ToList()
            ))
            .ToList();

            return permissionsByPage;
        }
    }
}
