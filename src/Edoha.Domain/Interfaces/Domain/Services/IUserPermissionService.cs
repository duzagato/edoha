using Action = Edoha.Domain.Entities.Action;
using Edoha.Domain.Entities;
using Edoha.Domain.Models.DTOs.UserPermission;

namespace Edoha.Domain.Interfaces.Domain.Services
{
    public interface IUserPermissionService : IService<UserPermission>
    {
        Task InsertUserPermission(CreateUserPermissionDTO dto);

        Task<UserPermission> SelectUserPermissionById(Guid id);

        Task<IEnumerable<UserPermission>> SelectAllUserPermissions();

        Task DeleteUserPermissionById(Guid id);

        Task<List<UserPermissionPage>> GetUserPermissionsGroupByPageName(Guid idUser);
        Task<Action?> GetUserActionByPageName(Guid idUser, string pageName, string actionName);
    }
}
