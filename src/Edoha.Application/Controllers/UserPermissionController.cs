using Edoha.Domain.Interfaces.Domain.Services;
using Edoha.Domain.Models.DTOs.UserPermission;
using Microsoft.AspNetCore.Mvc;

namespace Edoha.Application.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class UserPermissionController : ControllerBase
    {
        private readonly IUserPermissionService _userPermissionService;

        public UserPermissionController(IUserPermissionService userPermissionService)
        {
            _userPermissionService = userPermissionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userPermissions = await _userPermissionService.SelectAllUserPermissions();

            if (userPermissions == null || !userPermissions.Any())
            {
                return NoContent();
            }

            return Ok(userPermissions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userPermission = await _userPermissionService.SelectUserPermissionById(id);

            if (userPermission == null)
            {
                return NotFound();
            }

            return Ok(userPermission);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserPermissionDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Dados incompletos ou não enviados");
            }

            await _userPermissionService.InsertUserPermission(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(Guid id)
        {
            await _userPermissionService.DeleteUserPermissionById(id);
            return Ok();
        }
    }
}
