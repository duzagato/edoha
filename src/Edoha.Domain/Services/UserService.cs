using Edoha.Shared.Helpers;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Repositories;
using Edoha.Domain.Interfaces.Services;
using Edoha.Domain.Models.Requests.User;
using Edoha.Domain.Models.InputModels.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edoha.Domain.Interfaces.Util;

namespace Edoha.Domain.Services
{
    public class UserService : Service<User>, IUserService
    {
        private readonly ICrypto _crypto;
        private readonly IUserTypeRepository _userTypeRepository;

        public UserService(IUserRepository repository, ICrypto crypto, IUserTypeRepository userTypeRepository) : base(repository) 
        { 
            _crypto = crypto;
            _userTypeRepository = userTypeRepository;
        }

        public bool ValidateUserCredentials(string? username, string? password)
        {
            if ((username != null && password != null) && (username.Length > 0 && password.Length > 0))
            {
                return true;
            }else if ((username == null && password == null) || (username == "" && password == ""))
            {
                return false;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public async Task InsertUser(CreateUserInputModel model)
        {
            await _userTypeRepository.VerifyIdExist(model.IdUserType);

            byte[] hashedPassword = null;
            string? username = model.NicknameUser;
            string? unhashedPassword = model.UnhashedPassword;

            if (this.ValidateUserCredentials(username, unhashedPassword))
            {
                if(unhashedPassword?.Length >= 8 && unhashedPassword.Length <= 30)
                {
                    _crypto.SetUnhashedValue(unhashedPassword);
                    hashedPassword = _crypto.GetPBKDF2();
                }
                else
                {
                    throw new ArgumentException();
                }
            }

            CreateUserRequest request = new CreateUserRequest
            {
                NameUser = model.NameUser,
                PhoneUser = model.PhoneUser,
                NicknameUser = username,
                PasswordUser = hashedPassword,
                IdUserType = model.IdUserType
            };

            await this.Insert(request);
        }

        public async void InsertUserInformation()
        {

        }

        public async void InsertUserCredentials()
        {

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
