using Microsoft.AspNetCore.Mvc;
using Edoha.Domain.Interfaces.Services;
using Edoha.Domain.Entities;
using System.Net.Sockets;
using Edoha.Domain.Models.DTOs.User;
using Edoha.Domain.Models.InputModels;
using Edoha.Domain.Models.InputModels.User;

namespace Edoha.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _userService.SelectAllUsers();

                if (!users.Any())
                {
                    return NoContent();
                }

                return Ok(users);
            }catch(Exception ex)
            {
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
            try
            {
                var user = await _userService.SelectUserById(id);

                if (user == null)
                {
                    return NoContent();
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message
                });
            }
        }



        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserInputModel model)
        {
            if(model != null)
            {
                await _userService.InsertUser(model);
                return Ok();
            }
            else
            {
                return BadRequest("Dados incompletos ou não enviados");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateUserDTO request)
        {
            if (request != null)
            {
                try
                {
                    await _userService.UpdateUserById(request);
                    return Ok();
                }
                catch (Exception ex)
                {
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
                return BadRequest("Dados incompletos ou não enviados");
            }
        }

        [HttpDelete("{id}")]
        public async Task DeleteById(Guid id)
        {
            await _userService.DeleteUserById(id);
        }
    }
}
