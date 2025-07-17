using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Context;
using Edoha.Domain.Interfaces.Repositories;
using Edoha.Domain.Interfaces.Services;
using Edoha.Domain.Models.DTOs.UserInstitution;
using Edoha.Shared.Helpers;
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

        public UserInstitutionService(IUserInstitutionRepository repository,
            IRequestValidationContext requestValidationContext
            ) : base(repository, requestValidationContext) { }

        public async Task InsertUserInstitution(CreateUserInstitutionDTO dto)
        {
            await this.Insert(dto);
        }

        public async Task<UserInstitution> SelectUserInstitutionById(Guid id)
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

        public async Task DeleteUserInstitutionById(Guid id)
        {
            await this.DeleteById(id);
        }
    }
}
