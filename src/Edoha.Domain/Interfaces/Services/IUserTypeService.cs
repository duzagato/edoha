using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edoha.Domain.Entities;
using Edoha.Domain.Models.Requests.UserType;

namespace Edoha.Domain.Interfaces.Services
{
    public interface IUserTypeService : IService<UserType>
    {
        public Task InsertUserType(CreateUserTypeRequest request);

        public Task<UserType> SelectUserTypeById(int id);

        public Task<IEnumerable<UserType>> SelectAllUserTypes();

        public Task UpdateUserTypeById(UpdateUserTypeRequest request);

        public Task DeleteUserTypeById(int id);
    }
}
