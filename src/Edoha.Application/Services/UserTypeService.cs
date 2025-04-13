using Edoha.Application.Helpers;
using Edoha.Domain.DTOs.UserType;
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
    public class UserTypeService : Service<UserType>
    {

        public UserTypeService(IUserTypeRepository repository) : base(repository) { }

        public async Task InsertUserType(CreateUserTypeDTO dto)
        {
            await this.Insert(dto);
        }

        public async Task<UserType> SelectUserTypeById(int id)
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

        public async Task DeleteUserTypeById(int id)
        {
            await this.DeleteById(id);
        }
    }
}
