using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Domain.Services;
using Edoha.Domain.Interfaces.Infraestructure.Context;
using Edoha.Domain.Interfaces.Infraestructure.Services;
using Edoha.Domain.Interfaces.Infraestructure.Util;
using Edoha.Domain.Models.DTOs.Auth;
using Edoha.Domain.Models.DTOs.User;

namespace Edoha.Domain.Services
{
    public class AuthService : IAuthService
    {
        private readonly IJson _json;
        private readonly IRequestValidationContext _requestValidationContext;
        private readonly ITokenGenerationService _tokenGenerationService;
        private readonly IUserService _userService;
        private readonly IUserPermissionService _userPermissionService;
        public AuthService(
            IJson json,
            IRequestValidationContext requestValidationContext,
            ITokenGenerationService tokenGenerationService,
            IUserService userService,
            IUserPermissionService userPermissionService
        ) 
        {
            _json = json;
            _requestValidationContext = requestValidationContext;
            _tokenGenerationService = tokenGenerationService;
            _userService = userService;
            _userPermissionService = userPermissionService;
        }

        public async Task<string> Autenticate(CredentialsDTO credentials)
        {
            await _requestValidationContext.ValidateDTO(credentials);
            var user = await _userService.ValidateUserCredentials(credentials.Nickname!, credentials.Password!);
            
            var token = await GenerateToken(user);

            return token;
        }

        private async Task<string> GenerateToken(User user)
        {
            var userPermissions = await _userPermissionService.GetUserPermissionsGroupByPageName(user.Id);
            var permissionsJson = _json.Serialize(userPermissions);

            var token = _tokenGenerationService.GenerateToken(user, permissionsJson);

            return token;
        }
    }
}
