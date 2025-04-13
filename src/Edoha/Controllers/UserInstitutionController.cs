using Microsoft.AspNetCore.Mvc;
using Edoha.Application.Services;
using Edoha.Domain.DTOs.UserInstitution;
using Edoha.Domain.Entities;
using System.Net.Sockets;

namespace Edoha.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserInstitutionController : ControllerBase
    {
        private readonly UserInstitutionService _userInstitutionService;

        public UserInstitutionController(UserInstitutionService userInstitutionService)
        {
            _userInstitutionService = userInstitutionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var userInstitutions = await _userInstitutionService.SelectAllUserInstitutions();

                if (!userInstitutions.Any())
                {
                    return NoContent();
                }

                return Ok(userInstitutions);
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
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var userInstitution = await _userInstitutionService.SelectUserInstitutionById(id);

                if (userInstitution == null)
                {
                    return NoContent();
                }

                return Ok(userInstitution);
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
        public async Task<IActionResult> Create([FromBody] CreateUserInstitutionDTO dto)
        {
            if(dto != null)
            {
                try
                {
                    await _userInstitutionService.InsertUserInstitution(dto);
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

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateUserInstitutionDTO dto)
        {
            if (dto != null)
            {
                try
                {
                    await _userInstitutionService.UpdateUserInstitutionById(dto);
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
        public async Task DeleteById(int id)
        {
            await _userInstitutionService.DeleteUserInstitutionById(id);
        }
    }
}
