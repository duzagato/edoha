using Edoha.Shared.Helpers;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Repositories;
using Edoha.Domain.Interfaces.Services;
using Edoha.Domain.Models.Requests.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Services
{
    public class UserService : Service<User>, IUserService
    {

        public UserService(IUserRepository repository) : base(repository) { }

        public async Task InsertUser(CreateUserRequest request)
        {
            await this.Insert(request);
        }

        public async Task<User> SelectUserById(int id)
        {
            return await _repository.SelectById(id);
        }

        public async Task<IEnumerable<User>> SelectAllUsers()
        {
            return await _repository.SelectAll();
        }

        public async Task UpdateUserById(UpdateUserRequest request)
        {
            await this.Update(request);
        }

        public async Task DeleteUserById(int id)
        {
            await this.DeleteById(id);
        }
    }
}
