using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Infraestructure.Services;
using Edoha.Infraestructure.Constants;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Edoha.Infraestructure.Services
{
    public class TokenGenerationService : ITokenGenerationService
    {
        private readonly ILogger<ITokenGenerationService> _logger;
        private readonly ISecretsManagerService _secretsManagerService;

        public TokenGenerationService(
            ISecretsManagerService secretsManagerService,
            ILogger<ITokenGenerationService> logger
        )
        {
            _logger = logger;
            _secretsManagerService = secretsManagerService;
        }

        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Nickname!)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretsManagerService.GetJwtKeyAsync().Result));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: JwtSettings.Issuer,
                audience: JwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(JwtSettings.ExpiresInMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}
