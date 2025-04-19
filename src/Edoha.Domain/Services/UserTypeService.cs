using Edoha.Shared.Helpers;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Repositories;
using Edoha.Domain.Interfaces.Services;
using Edoha.Domain.Models.Requests.UserType;
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

        public UserTypeService(IUserTypeRepository repository) : base(repository) { }

        public async Task InsertUserType(CreateUserTypeRequest request)
        {
            await this.Insert(request);
        }

        public async Task<UserType> SelectUserTypeById(int id)
        {
            return await _repository.SelectById(id);
        }

        public async Task<IEnumerable<UserType>> SelectAllUserTypes()
        {
            return await _repository.SelectAll();
        }

        public async Task UpdateUserTypeById(UpdateUserTypeRequest request)
        {
            await this.Update(request);
        }

        public async Task DeleteUserTypeById(int id)
        {
            await this.DeleteById(id);
        }
    }
}
