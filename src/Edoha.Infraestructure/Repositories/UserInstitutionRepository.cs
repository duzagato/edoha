using Edoha.Domain.Entities;
using Edoha.Infrastructure.Repositories;
using System.Data;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;

namespace Edoha.Infraestructure.Repositories
{
    public class UserInstitutionRepository : BaseRepository<UserInstitution>, IUserInstitutionRepository
    {
        public UserInstitutionRepository(IDbConnection connection) : base(connection) 
        { 
            
        }

    }
}
