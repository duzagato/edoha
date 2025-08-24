using Edoha.Domain.Interfaces.Domain.Services;
using Edoha.Domain.Models.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Edoha.Application.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase // ← herda de ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> Autenticate([FromBody] CredentialsDTO credentials)
        {
            if (credentials != null)
            {
                var token = await _authService.Autenticate(credentials);

                return Ok(new
                {
                    token = token
                });
            }
            else
            {
                return BadRequest("Dados incompletos ou não enviados"); // ← BadRequest() também disponível
            }
        }
    }
}
