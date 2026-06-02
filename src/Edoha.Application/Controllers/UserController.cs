using Edoha.Domain.Interfaces.Domain.Services;
using Edoha.Domain.Models.DTOs.User;
using Edoha.Domain.Models.InputModels;
using Edoha.Domain.Models.InputModels.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Edoha.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserService userService, ILogger<UserController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Iniciando método GetAll (UserController)");
            
            try
            {
                _logger.LogInformation("Chamando _userService.SelectAllUsers()");
                var users = await _userService.SelectAllUsers();

                if (!users.Any())
                {
                    _logger.LogInformation("Nenhum usuário encontrado. Retornando NoContent");
                    return NoContent();
                }

                _logger.LogInformation("Retornando {Count} usuários encontrados", users.Count());
                return Ok(users);
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar GetAll: {Message}", ex.Message);
                return StatusCode(500, new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            _logger.LogInformation("Iniciando método GetById (UserController)");
            _logger.LogInformation("Parâmetros recebidos - id: {Id}", id);
            
            try
            {
                _logger.LogInformation("Chamando _userService.SelectUserById com id: {Id}", id);
                var user = await _userService.SelectUserById(id);

                if (user == null)
                {
                    _logger.LogInformation("Usuário com id {Id} não encontrado. Retornando NoContent", id);
                    return NoContent();
                }

                _logger.LogInformation("Usuário com id {Id} encontrado. Retornando dados do usuário", id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar GetById com id {Id}: {Message}", id, ex.Message);
                return StatusCode(500, new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message
                });
            }
        }

        [HttpGet("user_information")]
        public async Task<IActionResult> GetUsersWithTicketbooks([FromQuery] bool withTicketbooks = false)
        {
            _logger.LogInformation("Iniciando método GetUsersWithTicketbooks (UserController)");
            _logger.LogInformation("Parâmetros recebidos - withTicketbooks: {WithTicketbooks}", withTicketbooks);
            
            _logger.LogInformation("Chamando _userService.GetUserInformation com withTicketbooks: {WithTicketbooks}", withTicketbooks);
            var users = await _userService.GetUserInformation(withTicketbooks);
            
            _logger.LogInformation("Retornando {Count} usuários com informações", users.Count());
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserInputModel model)
        {
            _logger.LogInformation("Iniciando método Create (UserController)");
            _logger.LogInformation("Parâmetros recebidos - model: {@Model}", model);
            
            if(model != null)
            {
                _logger.LogInformation("Chamando _userService.InsertUser");
                await _userService.InsertUser(model);
                
                _logger.LogInformation("Usuário criado com sucesso. Retornando Ok");
                return Ok();
            }
            else
            {
                _logger.LogWarning("Dados incompletos ou não enviados no método Create");
                return BadRequest("Dados incompletos ou não enviados");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateUserDTO request)
        {
            _logger.LogInformation("Iniciando método Update (UserController)");
            _logger.LogInformation("Parâmetros recebidos - request: {@Request}", request);
            
            if (request != null)
            {
                try
                {
                    _logger.LogInformation("Chamando _userService.UpdateUserById");
                    await _userService.UpdateUserById(request);
                    
                    _logger.LogInformation("Usuário atualizado com sucesso. Retornando Ok");
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao executar Update: {Message}", ex.Message);
                    return StatusCode(500, new
                    {
                        Message = ex.Message,
                        StackTrace = ex.StackTrace,
                        InnerException = ex.InnerException?.Message
                    });
                }
            }
            else
            {
                _logger.LogWarning("Dados incompletos ou não enviados no método Update");
                return BadRequest("Dados incompletos ou não enviados");
            }
        }

        [HttpDelete("{id}")]
        public async Task DeleteById(Guid id)
        {
            _logger.LogInformation("Iniciando método DeleteById (UserController)");
            _logger.LogInformation("Parâmetros recebidos - id: {Id}", id);
            
            _logger.LogInformation("Chamando _userService.DeleteUserById com id: {Id}", id);
            await _userService.DeleteUserById(id);
            
            _logger.LogInformation("Usuário com id {Id} deletado com sucesso", id);
        }
    }
}
