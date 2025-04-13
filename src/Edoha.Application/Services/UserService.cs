using Edoha.Application.Helpers;
using Edoha.Domain.DTOs.User;
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
    public class UserService : Service<User>
    {

        public UserService(IUserRepository repository) : base(repository) { }

        public async Task InsertUser(CreateUserDTO dto)
        {
            await this.Insert(dto);
        }

        public async Task<User> SelectUserById(int id)
        {
            return await _repository.SelectById(id);
        }

        public async Task<IEnumerable<User>> SelectAllUsers()
        {
            return await _repository.SelectAll();
        }

        public async Task UpdateUserById(UpdateUserDTO dto)
        {
            await this.Update(dto);
        }

        public async Task DeleteUserById(int id)
        {
            await this.DeleteById(id);
        }
    }
}
