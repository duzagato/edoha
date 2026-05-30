using Edoha.Domain.Constants.Alerts;
using Edoha.Domain.Entities;
using Edoha.Domain.Exceptions;
using Edoha.Domain.Interfaces.Domain.Services;
using Edoha.Domain.Interfaces.Infraestructure.Context;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Interfaces.Infraestructure.Util;
using Edoha.Domain.Models.DTOs.User;
using Edoha.Domain.Models.InputModels.User;
using Edoha.Domain.Models.Responses.User;
using Microsoft.Extensions.Logging;

namespace Edoha.Domain.Services
{
    public class UserService : Service<User>, IUserService
    {
        private readonly ICrypto _crypto;
        private readonly ILogger<IUserService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IUserTypeRepository _userTypeRepository;

        public UserService(IUserRepository repository, 
            ICrypto crypto,
            ILogger<IUserService> logger,
            IUserRepository userRepository,
            IUserTypeRepository userTypeRepository, 
            IRequestValidationContext requestValidationContext) 
            : base(repository, requestValidationContext) 
        { 
            _crypto = crypto;
            _logger = logger;
            _userRepository = userRepository;
            _userTypeRepository = userTypeRepository;
        }

        public async Task InsertUser(CreateUserInputModel model)
        {
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
                Password = hashedPassword
            };

            await Insert(dto);
        }

        public async Task<Guid> InsertUserInformation(string? name, string? phone)
        {
            if (!String.IsNullOrEmpty(name) &&
               !String.IsNullOrEmpty(phone))
            {
                UserInformation userInformation = new UserInformation()
                {
                    Name = name,
                    Phone = phone
                };

                return await _repository.InsertOrGetId(userInformation);
            }
            else
            {
                throw new ArgumentException("Nome e Telefone são obrigatórios.");
            }
        }

        public async Task<IEnumerable<UserInformationResponse?>> GetUserInformation(bool withTicketbooks)
        {
            var users = await _userRepository.SelectUserInformation(withTicketbooks);

            return users;
        }

        public void InsertUserCredentials()
        {

        }

        public async Task<User?> SelectUserCredentialsByNickname(string nickname)
        {
            var user = await _userRepository.SelectUserCredentialsByNickname(nickname);

            return user;
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

        public async Task<User> ValidateUserCredentials(string nickname, string password)
        {
            _logger.LogInformation("Validando credenciais do usuário");
            _logger.LogInformation("Procurando usuário usando o nickname: {Nickname}", nickname);

            var user = await _userRepository.SelectUserCredentialsByNickname(nickname);

            if (user is null)
            {
                _logger.LogError("Usuário recebido foi nulo");
                SetInvalidCredentialsMessage();
                throw new RequestValidationException(_requestValidationContext.GetErrors());
            }

            bool isValidPassword = _crypto.ValidatePBKDF2(password, user.Password!);

            if (!isValidPassword)
            {
                _logger.LogError("Senha inválida");
                SetInvalidCredentialsMessage();
                throw new RequestValidationException(_requestValidationContext.GetErrors());
            }

            _logger.LogInformation("Usuário válido");

            return user;
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

        private void SetInvalidCredentialsMessage()
        {
            _requestValidationContext.AddError("Autentication", "Usuário ou Senha inválido!");
        }
    }
}
