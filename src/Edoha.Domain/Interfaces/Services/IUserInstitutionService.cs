using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edoha.Domain.Entities;
using Edoha.Domain.Models.DTOs.UserInstitution;

namespace Edoha.Domain.Interfaces.Services
{
    public interface IUserInstitutionService : IService<UserInstitution>
    {
        public Task InsertUserInstitution(CreateUserInstitutionDTO dto);

        public Task<UserInstitution> SelectUserInstitutionById(Guid id);

        public Task<IEnumerable<UserInstitution>> SelectAllUserInstitutions();

        public Task UpdateUserInstitutionById(UpdateUserInstitutionDTO dto);

        public Task DeleteUserInstitutionById(Guid id);
    }
}
