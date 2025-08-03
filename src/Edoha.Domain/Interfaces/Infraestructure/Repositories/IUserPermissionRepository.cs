using Edoha.Domain.Entities;
using Edoha.Domain.Models.DTOs.UserPermission;

namespace Edoha.Domain.Interfaces.Infraestructure.Repositories
{
    public interface IUserPermissionRepository : IBaseRepository<UserPermission>
    {
        Task<IEnumerable<UserPermissionExpand>> GetUserPermissionExpandByIdUser(Guid idUser);
    }
}
