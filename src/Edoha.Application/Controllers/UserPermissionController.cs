using Edoha.Domain.Interfaces.Domain.Services;
using Edoha.Domain.Models.DTOs.UserPermission;
using Microsoft.AspNetCore.Mvc;

namespace Edoha.Application.Controllers
{
    [ApiController]
    [Route("/userpermission")]
    public class UserPermissionController : ControllerBase
    {
        private readonly ILogger _ilogger;
        private readonly IUserPermissionService _userPermissionService;

        public UserPermissionController(ILogger ilogger, IUserPermissionService userPermissionService)
        {
            _ilogger = ilogger;
            _userPermissionService = userPermissionService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userPermission = await _userPermissionService.GetUserPermissionsGroupByPageName(id);

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
