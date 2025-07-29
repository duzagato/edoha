using Edoha.Domain.Entities;
using Edoha.Infrastructure.Repositories;
using System.Data;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;

namespace Edoha.Infraestructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(IDbConnection connection) : base(connection) 
        { 
            
        }

    }
}
