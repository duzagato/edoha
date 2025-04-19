using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edoha.Domain.Entities;
using Edoha.Domain.Models.Requests.UserInstitution;

namespace Edoha.Domain.Interfaces.Services
{
    public interface IUserInstitutionService : IService<UserInstitution>
    {
        public Task InsertUserInstitution(CreateUserInstitutionRequest request);

        public Task<UserInstitution> SelectUserInstitutionById(int id);

        public Task<IEnumerable<UserInstitution>> SelectAllUserInstitutions();

        public Task UpdateUserInstitutionById(UpdateUserInstitutionRequest request);

        public Task DeleteUserInstitutionById(int id);
    }
}
