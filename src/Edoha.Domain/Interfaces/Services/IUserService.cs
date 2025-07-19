using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edoha.Domain.Entities;
using Edoha.Domain.Models.InputModels.User;
using Edoha.Domain.Models.DTOs.User;

namespace Edoha.Domain.Interfaces.Services
{
    public interface IUserService : IService<User>
    {
        public Task InsertUser(CreateUserInputModel model);

        public Task<User> SelectUserById(Guid id);

        public Task<IEnumerable<User>> SelectAllUsers();

        public Task UpdateUserById(UpdateUserDTO dto);

        public Task DeleteUserById(Guid id);
    }
}
