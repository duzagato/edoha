using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Context;
using Edoha.Domain.Interfaces.Repositories;
using Edoha.Domain.Interfaces.Services;
using Edoha.Domain.Models.DTOs.UserType;
using Edoha.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Services
{
    public class UserTypeService : Service<UserType>, IUserTypeService
    {

        public UserTypeService(IUserTypeRepository repository,
            IRequestValidationContext requestValidationContext
            ) : base(repository, requestValidationContext) { }

        public async Task InsertUserType(CreateUserTypeDTO dto)
        {
            await this.Insert(dto);
        }

        public async Task<UserType> SelectUserTypeById(Guid id)
        {
            return await _repository.SelectById(id);
        }

        public async Task<IEnumerable<UserType>> SelectAllUserTypes()
        {
            return await _repository.SelectAll();
        }

        public async Task UpdateUserTypeById(UpdateUserTypeDTO dto)
        {
            await this.Update(dto);
        }

        public async Task DeleteUserTypeById(Guid id)
        {
            await this.DeleteById(id);
        }
    }
}
