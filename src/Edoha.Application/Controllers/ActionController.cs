using Edoha.Domain.Interfaces.Domain.Services;
using Edoha.Domain.Models.DTOs.Action;
using Microsoft.AspNetCore.Mvc;

namespace Edoha.Application.Controllers
{
    [ApiController]
    [Route("/[controller]")]
    public class ActionController : ControllerBase
    {
        private readonly IActionService _actionService;

        public ActionController(IActionService actionService)
        {
            _actionService = actionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var actions = await _actionService.SelectAllActions();

            if (actions == null || !actions.Any())
            {
                return NoContent();
            }

            return Ok(actions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var action = await _actionService.SelectActionById(id);

            if (action == null)
            {
                return NotFound();
            }

            return Ok(action);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateActionDTO request)
        {
            if (request == null)
            {
                return BadRequest("Dados incompletos ou não enviados");
            }

            await _actionService.InsertAction(request);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateActionDTO request)
        {
            if (request == null)
            {
                return BadRequest("Dados incompletos ou não enviados");
            }

            await _actionService.UpdateActionById(request);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteById(Guid id)
        {
            await _actionService.DeleteActionById(id);
            return Ok();
        }
    }
}
