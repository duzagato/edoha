using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Entities;
using Edoha.Infrastructure.Repositories;
using System.Data;

namespace Edoha.Infraestructure.Repositories
{
    public class UserTypeRepository : BaseRepository<UserType>, IUserTypeRepository
    {
        public UserTypeRepository(IDbConnection connection) : base(connection) 
        { 
            
        }

    }
}
