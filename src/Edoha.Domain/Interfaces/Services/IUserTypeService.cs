using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edoha.Domain.Entities;
using Edoha.Domain.Models.DTOs.UserType;

namespace Edoha.Domain.Interfaces.Services
{
    public interface IUserTypeService : IService<UserType>
    {
        public Task InsertUserType(CreateUserTypeDTO dto);

        public Task<UserType> SelectUserTypeById(Guid id);

        public Task<IEnumerable<UserType>> SelectAllUserTypes();

        public Task UpdateUserTypeById(UpdateUserTypeDTO dto);

        public Task DeleteUserTypeById(Guid id);
    }
}
