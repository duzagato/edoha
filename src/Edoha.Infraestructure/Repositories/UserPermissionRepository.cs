using Dapper;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Infraestructure.Factories;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Models.DTOs.UserPermission;
using Edoha.Infraestructure.Constants;
using Edoha.Infrastructure.Repositories;
using System.Data;

namespace Edoha.Infraestructure.Repositories
{
    public class UserPermissionRepository : BaseRepository<UserPermission>, IUserPermissionRepository
    {
        public UserPermissionRepository(IDbConnectionFactory connectionFactory )
        : base(connectionFactory) { }

        public async Task<IEnumerable<UserPermissionExpand>> GetUserPermissionExpandByIdUser(Guid idUser)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            CheckConnection();

            string query = StaticQueries.GetUserPermissionExpandByIdUser;

            var userPermissions= await _connection.QueryAsync<UserPermissionExpand>(query, new { IdUser = idUser });

            return userPermissions ?? throw new KeyNotFoundException("Entidade não encontrada");
        }
    }
}
