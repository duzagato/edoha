using Edoha.Application.Helpers;
using Edoha.Domain.DTOs.UserInstitution;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Application.Services
{
    public class UserInstitutionService : Service<UserInstitution>
    {

        public UserInstitutionService(IUserInstitutionRepository repository) : base(repository) { }

        public async Task InsertUserInstitution(CreateUserInstitutionDTO dto)
        {
            await this.Insert(dto);
        }

        public async Task<UserInstitution> SelectUserInstitutionById(int id)
        {
            return await _repository.SelectById(id);
        }

        public async Task<IEnumerable<UserInstitution>> SelectAllUserInstitutions()
        {
            return await _repository.SelectAll();
        }

        public async Task UpdateUserInstitutionById(UpdateUserInstitutionDTO dto)
        {
            await this.Update(dto);
        }

        public async Task DeleteUserInstitutionById(int id)
        {
            await this.DeleteById(id);
        }
    }
}
