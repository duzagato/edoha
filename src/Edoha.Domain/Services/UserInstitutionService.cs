using Edoha.Shared.Helpers;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Repositories;
using Edoha.Domain.Interfaces.Services;
using Edoha.Domain.Models.Requests.UserInstitution;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Services
{
    public class UserInstitutionService : Service<UserInstitution>, IUserInstitutionService
    {

        public UserInstitutionService(IUserInstitutionRepository repository) : base(repository) { }

        public async Task InsertUserInstitution(CreateUserInstitutionRequest request)
        {
            await this.Insert(request);
        }

        public async Task<UserInstitution> SelectUserInstitutionById(int id)
        {
            return await _repository.SelectById(id);
        }

        public async Task<IEnumerable<UserInstitution>> SelectAllUserInstitutions()
        {
            return await _repository.SelectAll();
        }

        public async Task UpdateUserInstitutionById(UpdateUserInstitutionRequest request)
        {
            await this.Update(request);
        }

        public async Task DeleteUserInstitutionById(int id)
        {
            await this.DeleteById(id);
        }
    }
}
