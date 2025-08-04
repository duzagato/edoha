using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Domain.Services;
using Edoha.Domain.Interfaces.Infraestructure.Services;
using Edoha.Domain.Interfaces.Infraestructure.Util;
using Edoha.Domain.Models.DTOs.Auth;

namespace Edoha.Domain.Services
{
    public class AuthService : IAuthService
    {
        private readonly IJson _json;
        public readonly ITokenGenerationService _tokenGenerationService;
        public readonly IUserService _userService;
        public readonly IUserPermissionService _userPermissionService;
        public AuthService(
            IJson json,
            ITokenGenerationService tokenGenerationService,
            IUserService userService,
            IUserPermissionService userPermissionService
        ) 
        {
            _json = json;
            _tokenGenerationService = tokenGenerationService;
            _userService = userService;
            _userPermissionService = userPermissionService;
        }

        public async Task<string> Authenticate(CredentialsDTO credentials)
        {
            User user = await _userService.SelectUserById(credentials.IdUser!);
            
            var userPermissions = await _userPermissionService.GetUserPermissionsGroupByPageName(credentials.IdUser!);
            var permissionsJson = _json.Serialize(userPermissions);

            var token = _tokenGenerationService.GenerateToken(user, permissionsJson);

            return token;
        }
    }
}
