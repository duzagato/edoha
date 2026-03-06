using Edoha.Domain.Interfaces.Domain.Services;
using Edoha.Domain.Models.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Edoha.Application.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase // ← herda de ControllerBase
    {
        private readonly ILogger<AuthController> _ilogger;
        private readonly IAuthService _authService;

        public AuthController(ILogger<AuthController> ilogger, IAuthService authService)
        {
            _ilogger = ilogger;
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> Autenticate([FromBody] CredentialsDTO credentials)
        {
            _ilogger.LogInformation("Iniciando geração de token");
            if (credentials != null)
            {
                _ilogger.LogInformation("Credenciais recebidas");
                var token = await _authService.Autenticate(credentials);
                _ilogger.LogInformation("Token gerado com sucesso!");

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
