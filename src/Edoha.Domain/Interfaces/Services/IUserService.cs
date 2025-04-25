using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edoha.Domain.Entities;
using Edoha.Domain.Models.InputModels.User;
using Edoha.Domain.Models.Requests.User;

namespace Edoha.Domain.Interfaces.Services
{
    public interface IUserService : IService<User>
    {
        public Task InsertUser(CreateUserInputModel request);

        public Task<User> SelectUserById(int id);

        public Task<IEnumerable<User>> SelectAllUsers();

        public Task UpdateUserById(UpdateUserRequest request);

        public Task DeleteUserById(int id);
    }
}
