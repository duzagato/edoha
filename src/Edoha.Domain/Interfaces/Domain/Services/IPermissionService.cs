using Edoha.Domain.Entities;
using Edoha.Domain.Models.DTOs.Permission;

namespace Edoha.Domain.Interfaces.Domain.Services
{
    public interface IPermissionService : IService<Permission>
    {
        Task<IEnumerable<Permission>> SelectAllPermissions();
        Task<Permission?> SelectPermissionById(Guid id);
        Task InsertPermission(CreatePermissionDTO dto);
        Task UpdatePermissionById(UpdatePermissionDTO dto);
        Task DeletePermissionById(Guid id);
    }
}
