using Edoha.Domain.Interfaces.Domain.Services;
using Edoha.Domain.Models.DTOs.Permission;
using Microsoft.AspNetCore.Mvc;

namespace Edoha.Application.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var permissions = await _permissionService.SelectAllPermissions();

            if (permissions == null || !permissions.Any())
            {
                return NoContent();
            }

            return Ok(permissions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var permission = await _permissionService.SelectPermissionById(id);

            if (permission == null)
            {
                return NotFound();
            }

            return Ok(permission);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePermissionDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Dados incompletos ou não enviados");
            }

            await _permissionService.InsertPermission(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdatePermissionDTO dto)
        {
            if (dto == null)
            {
                return BadRequest("Dados incompletos ou não enviados");
            }

            await _permissionService.UpdatePermissionById(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(Guid id)
        {
            await _permissionService.DeletePermissionById(id);
            return Ok();
        }
    }
}
