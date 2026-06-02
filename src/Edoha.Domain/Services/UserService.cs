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
            : base(repository, requestValidationContext, logger) 
        { 
            _crypto = crypto;
            _logger = logger;
            _userRepository = userRepository;
            _userTypeRepository = userTypeRepository;
        }

        public async Task InsertUser(CreateUserInputModel model)
        {
            _logger.LogInformation("Iniciando método InsertUser (UserService)");
            _logger.LogInformation("Parâmetros recebidos - model: {@Model}", model);
            
            byte[]? hashedPassword = null;
            bool usernameSended = IsUsernameSended(model.Nickname);
            bool passwordSended = IsPasswordSended(model.UnhashedPassword);

            _logger.LogInformation("Verificando se username e password foram enviados - usernameSended: {UsernameSended}, passwordSended: {PasswordSended}", usernameSended, passwordSended);

            if (usernameSended && passwordSended)
            {
                _logger.LogInformation("Realizando hash da senha");
                hashedPassword = HashPassword(model.UnhashedPassword!);
            }
            else
            {
                _logger.LogWarning("Username ou password não foram enviados. Configurando mensagens de validação");
                SetValidationMessages(usernameSended, passwordSended);
            }

            CreateUserDTO dto = new CreateUserDTO
            {
                Name = model.Name!,
                Phone = model.Phone!,
                Nickname = model.Nickname,
                Password = hashedPassword
            };

            _logger.LogInformation("Chamando método Insert com dto criado");
            await Insert(dto);
            
            _logger.LogInformation("Método InsertUser finalizado com sucesso");
        }

        public async Task<Guid> InsertUserInformation(string? name, string? phone)
        {
            _logger.LogInformation("Iniciando método InsertUserInformation");
            _logger.LogInformation("Parâmetros recebidos - name: {Name}, phone: {Phone}", name, phone);
            
            if (!String.IsNullOrEmpty(name) &&
               !String.IsNullOrEmpty(phone))
            {
                UserInformation userInformation = new UserInformation()
                {
                    Name = name,
                    Phone = phone
                };

                var userId = await _repository.InsertOrGetId(userInformation);
                
                _logger.LogInformation("Método InsertUserInformation finalizado. Retornando Id: {UserId}", userId);
                return userId;
            }
            else
            {
                _logger.LogError("Nome e Telefone são obrigatórios mas foram fornecidos vazios ou nulos. name: {Name}, phone: {Phone}", name, phone);
                throw new ArgumentException("Nome e Telefone são obrigatórios.");
            }
        }

        public async Task<IEnumerable<UserInformationResponse?>> GetUserInformation(bool withTicketbooks)
        {
            _logger.LogInformation("Iniciando método GetUserInformation (UserService)");
            _logger.LogInformation("Parâmetros recebidos - withTicketbooks: {WithTicketbooks}", withTicketbooks);
            
            _logger.LogInformation("Chamando _userRepository.SelectUserInformation");
            var users = await _userRepository.SelectUserInformation(withTicketbooks);

            _logger.LogInformation("Método GetUserInformation finalizado. Retornando {Count} usuários", users.Count());
            return users;
        }

        public void InsertUserCredentials()
        {
            _logger.LogInformation("Método InsertUserCredentials (UserService) chamado - método vazio");
        }

        public async Task<User?> SelectUserCredentialsByNickname(string nickname)
        {
            _logger.LogInformation("Iniciando método SelectUserCredentialsByNickname (UserService)");
            _logger.LogInformation("Parâmetros recebidos - nickname: {Nickname}", nickname);
            
            _logger.LogInformation("Chamando _userRepository.SelectUserCredentialsByNickname");
            var user = await _userRepository.SelectUserCredentialsByNickname(nickname);

            if (user == null)
            {
                _logger.LogInformation("Usuário com nickname {Nickname} não encontrado. Retornando null", nickname);
            }
            else
            {
                _logger.LogInformation("Usuário com nickname {Nickname} encontrado. Retornando resultado", nickname);
            }
            
            return user;
        }

        public async Task<User> SelectUserById(Guid id)
        {
            _logger.LogInformation("Iniciando método SelectUserById (UserService)");
            _logger.LogInformation("Parâmetros recebidos - id: {Id}", id);
            
            _logger.LogInformation("Chamando _repository.SelectById");
            var user = await _repository.SelectById(id);
            
            _logger.LogInformation("Método SelectUserById finalizado. Retornando usuário com id {Id}", id);
            return user;
        }

        public async Task<IEnumerable<User>> SelectAllUsers()
        {
            _logger.LogInformation("Iniciando método SelectAllUsers (UserService)");
            
            _logger.LogInformation("Chamando _repository.SelectAll");
            var users = await _repository.SelectAll();
            
            _logger.LogInformation("Método SelectAllUsers finalizado. Retornando {Count} usuários", users.Count());
            return users;
        }

        public async Task UpdateUserById(UpdateUserDTO dto)
        {
            _logger.LogInformation("Iniciando método UpdateUserById (UserService)");
            _logger.LogInformation("Parâmetros recebidos - dto: {@Dto}", dto);
            
            _logger.LogInformation("Chamando método Update da classe base Service");
            await Update(dto);
            
            _logger.LogInformation("Método UpdateUserById finalizado com sucesso");
        }

        public async Task DeleteUserById(Guid id)
        {
            _logger.LogInformation("Iniciando método DeleteUserById (UserService)");
            _logger.LogInformation("Parâmetros recebidos - id: {Id}", id);
            
            _logger.LogInformation("Chamando método DeleteById da classe base Service");
            await DeleteById(id);
            
            _logger.LogInformation("Método DeleteUserById finalizado com sucesso para id {Id}", id);
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
            _logger.LogInformation("Iniciando método IsUsernameSended (UserService)");
            _logger.LogInformation("Parâmetros recebidos - username: {Username}", username);
            
            if (!String.IsNullOrWhiteSpace(username))
            {
                _logger.LogInformation("Username válido. Retornando true");
                return true;
            }
            else
            {
                _logger.LogInformation("Username inválido ou vazio. Retornando false");
                return false;
            }
        }

        private bool IsPasswordSended(string? password)
        {
            _logger.LogInformation("Iniciando método IsPasswordSended (UserService)");
            _logger.LogInformation("Parâmetros recebidos - password: {PasswordMasked}", password != null ? "***" : "null");
            
            if (!String.IsNullOrWhiteSpace(password))
            {
                _logger.LogInformation("Password válido. Retornando true");
                return true;
            }
            else
            {
                _logger.LogInformation("Password inválido ou vazio. Retornando false");
                return false;
            }
        }

        private byte[]? HashPassword(string unhashedPassword)
        {
            _logger.LogInformation("Iniciando método HashPassword (UserService)");
            _logger.LogInformation("Comprimento da senha recebida: {Length}", unhashedPassword.Length);
            
            if (unhashedPassword.Length >= 8 && unhashedPassword.Length <= 30)
            {
                _logger.LogInformation("Senha válida. Gerando hash PBKDF2");
                _crypto.SetUnhashedValue(unhashedPassword);
                var hashedPassword = _crypto.GetPBKDF2();
                
                _logger.LogInformation("Hash gerado com sucesso. Retornando senha hasheada");
                return hashedPassword;
            }
            else
            {
                _logger.LogWarning("Senha com comprimento inválido: {Length}. Deve estar entre 8 e 30 caracteres", unhashedPassword.Length);
                _requestValidationContext.AddError("UnhashedPassword", UserAlerts.InvalidPasswordLength);
                return null;
            }
        }

        private void SetValidationMessages(bool usernameSended, bool passwordSended)
        {
            _logger.LogInformation("Iniciando método SetValidationMessages (UserService)");
            _logger.LogInformation("Parâmetros recebidos - usernameSended: {UsernameSended}, passwordSended: {PasswordSended}", usernameSended, passwordSended);
            
            if (!usernameSended)
            {
                _logger.LogWarning("Adicionando erro de validação: Username vazio");
                _requestValidationContext.AddError("Nickname", UserAlerts.EmptyUsername);
            }

            if (!passwordSended)
            {
                _logger.LogWarning("Adicionando erro de validação: Password vazio");
                _requestValidationContext.AddError("UnhashedPassword", UserAlerts.EmptyPassword);
            }
            
            _logger.LogInformation("Método SetValidationMessages finalizado");
        }

        private void SetInvalidCredentialsMessage()
        {
            _logger.LogInformation("Iniciando método SetInvalidCredentialsMessage (UserService)");
            _logger.LogWarning("Adicionando erro de validação: Credenciais inválidas");
            _requestValidationContext.AddError("Autentication", "Usuário ou Senha inválido!");
        }
    }
}
