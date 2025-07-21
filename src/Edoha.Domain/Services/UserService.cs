using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Context;
using Edoha.Domain.Interfaces.Repositories;
using Edoha.Domain.Interfaces.Services;
using Edoha.Domain.Interfaces.Util;
using Edoha.Domain.Models.DTOs.User;
using Edoha.Domain.Models.InputModels.User;
using Edoha.Shared.Constants.Alerts;
using Edoha.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Services
{
    public class UserService : Service<User>, IUserService
    {
        private readonly ICrypto _crypto;
        private readonly IUserTypeRepository _userTypeRepository;

        public UserService(IUserRepository repository, 
            ICrypto crypto, 
            IUserTypeRepository userTypeRepository, 
            IRequestValidationContext requestValidationContext) 
            : base(repository, requestValidationContext) 
        { 
            _crypto = crypto;
            _userTypeRepository = userTypeRepository;
        }

        public async Task InsertUser(CreateUserInputModel model)
        {
            await _userTypeRepository.IdExists(model.IdUserType);

            byte[]? hashedPassword = null;
            bool usernameSended = IsUsernameSended(model.Nickname);
            bool passwordSended = IsPasswordSended(model.UnhashedPassword);

            if (usernameSended && passwordSended)
            {
                hashedPassword = HashPassword(model.UnhashedPassword!);
            }
            else
            {
                SetValidationMessages(usernameSended, passwordSended);
            }

            CreateUserDTO dto = new CreateUserDTO
            {
                Name = model.Name!,
                Phone = model.Phone!,
                Nickname = model.Nickname,
                Password = hashedPassword,
                IdUserType = model.IdUserType
            };

            await Insert(dto);
        }

        public void InsertUserInformation()
        {

        }

        public void InsertUserCredentials()
        {

        }

        public async Task<User> SelectUserById(Guid id)
        {
            return await _repository.SelectById(id);
        }

        public async Task<IEnumerable<User>> SelectAllUsers()
        {
            return await _repository.SelectAll();
        }

        public async Task UpdateUserById(UpdateUserDTO dto)
        {
            await Update(dto);
        }

        public async Task DeleteUserById(Guid id)
        {
            await DeleteById(id);
        }

        private bool IsUsernameSended(string? username)
        {
            if (!String.IsNullOrWhiteSpace(username))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsPasswordSended(string? password)
        {
            if (!String.IsNullOrWhiteSpace(password))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private byte[]? HashPassword(string unhashedPassword)
        {
            if (unhashedPassword.Length >= 8 && unhashedPassword.Length <= 30)
            {
                _crypto.SetUnhashedValue(unhashedPassword);
                return _crypto.GetPBKDF2();
            }
            else
            {
                _requestValidationContext.AddError("UnhashedPassword", UserAlerts.InvalidPasswordLength);
                return null;
            }
        }

        private void SetValidationMessages(bool usernameSended, bool passwordSended)
        {
            if (!usernameSended)
            {
                _requestValidationContext.AddError("Nickname", UserAlerts.EmptyUsername);
            }

            if (!passwordSended)
            {
                _requestValidationContext.AddError("UnhashedPassword", UserAlerts.EmptyPassword);
            }
        }
    }
}
