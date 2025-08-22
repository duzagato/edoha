using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Infraestructure.Services;
using Edoha.Infraestructure.Constants;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Edoha.Infraestructure.Services
{
    public class TokenGenerationService : ITokenGenerationService
    {
        public string GenerateToken(User user, string permissionsJson)
        {

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Nickname!),
                new Claim("Permissions", permissionsJson)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(JwtConfig.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: JwtConfig.Issuer,
                audience: JwtConfig.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(JwtConfig.ExpirationMinutes)),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
