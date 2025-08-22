using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Domain.Services;
using Edoha.Domain.Interfaces.Infraestructure.Context;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Interfaces.Infraestructure.Services;
using Edoha.Domain.Interfaces.Infraestructure.Util;
using Edoha.Domain.Models.DTOs.Auth;
using Edoha.Domain.Models.DTOs.User;
using Edoha.Domain.Models.Login;

namespace Edoha.Domain.Services
{
    public class AuthService : IAuthService
    {
        private readonly IJson _json;
        private readonly ILoginRepository _loginRepository;
        private readonly IRequestValidationContext _requestValidationContext;
        private readonly ISystemUtils _systemUtils;
        private readonly ITokenGenerationService _tokenGenerationService;
        private readonly IUserService _userService;
        private readonly IUserPermissionService _userPermissionService;

        public AuthService(
            IJson json,
            ILoginRepository loginRepository,
            IRequestValidationContext requestValidationContext,
            ISystemUtils systemUtils,
            ITokenGenerationService tokenGenerationService,
            IUserService userService,
            IUserPermissionService userPermissionService
        ) 
        {
            _json = json;
            _loginRepository = loginRepository;
            _requestValidationContext = requestValidationContext;
            _systemUtils = systemUtils;
            _tokenGenerationService = tokenGenerationService;
            _userService = userService;
            _userPermissionService = userPermissionService;
        }

        public async Task<string> Autenticate(CredentialsDTO credentials)
        {
            await _requestValidationContext.ValidateDTO(credentials);
            var user = await _userService.ValidateUserCredentials(credentials.Nickname!, credentials.Password!);
            
            var token = await GenerateToken(user);
            await InsertLoginInformation(user!);
            

            return token;
        }

        private async Task<string> GenerateToken(User user)
        {
            var userPermissions = await _userPermissionService.GetUserPermissionsGroupByPageName(user.Id);
            var permissionsJson = _json.Serialize(userPermissions);

            var token = _tokenGenerationService.GenerateToken(user, permissionsJson);

            return token;
        }

        private async Task InsertLoginInformation(User user)
        {
            string ip = _systemUtils.GetClientIp();
            var refreshToken = _tokenGenerationService.GenerateRefreshToken();

            var loginInformations = new InsertLoginInformationsWithoutTokenExpiration
            {
                IdUser = user.Id,
                RefreshTokenHash = refreshToken,
                Ip = ip
            };

            await _requestValidationContext.ValidateDTO(loginInformations);
            await _loginRepository.Insert(loginInformations);


        }
    }
}
