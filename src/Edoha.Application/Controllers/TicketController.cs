using Microsoft.AspNetCore.Mvc;
using Edoha.Domain.Models.DTOs.Ticket;
using Edoha.Domain.Interfaces.Domain.Services;
using Edoha.Domain.Models.Requests.Ticket;

namespace Edoha.Controllers
{
    [ApiController]
    [Route("ticket")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tickets = await _ticketService.SelectAllTickets();

            if (!tickets.Any())
            {
                return NoContent();
            }

            return Ok(tickets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var ticket = await _ticketService.SelectTicketById(id);

            if (ticket == null)
            {
                return NoContent();
            }

            return Ok(ticket);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTicketRequest request)
        {
            if (request != null)
            {
                await _ticketService.InsertTicket(request);
                return Created();
            }
            else
            {
                return BadRequest("Dados incompletos ou não enviados");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateTicketDTO request)
        {
            if (request != null)
            {
                await _ticketService.UpdateTicketById(request);
                return Ok();
            }
            else
            {
                return BadRequest("Dados incompletos ou não enviados");
            }
        }

        [HttpDelete("{id}")]
        public async Task DeleteById(Guid id)
        {
            await _ticketService.DeleteTicketById(id);
        }
    }
}
