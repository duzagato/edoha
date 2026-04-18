using Microsoft.AspNetCore.Mvc;
using Edoha.Domain.Models.DTOs.Ticket;
using Edoha.Domain.Interfaces.Domain.Services;
using Edoha.Domain.Models.Requests.Ticket;

namespace Edoha.Controllers
{
    [ApiController]
    [Route("ticketbook/{idTicketbook}/ticket")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid idTicketbook)
        {
            var tickets = await _ticketService.SelectAllTickets(idTicketbook);

            if (!tickets.Any())
            {
                return NoContent();
            }

            return Ok(tickets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid idTicketbook, Guid id)
        {
            var ticket = await _ticketService.SelectTicketById(idTicketbook, id);

            if (ticket == null)
            {
                return NoContent();
            }

            return Ok(ticket);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid idTicketbook, [FromBody] List<CreateTicketRequest> request)
        {
            if (request != null && request.Count > 0)
            {
                foreach(var ticket in request)
                {
                    await _ticketService.InsertTicket(idTicketbook, ticket);
                }

                return Created();
            }
            else
            {
                return BadRequest("Dados incompletos ou não enviados");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(Guid idTicketbook, [FromBody] UpdateTicketDTO request)
        {
            if (request != null)
            {
                await _ticketService.UpdateTicketById(idTicketbook, request);
                return Ok();
            }
            else
            {
                return BadRequest("Dados incompletos ou não enviados");
            }
        }

        [HttpDelete("{id}")]
        public async Task DeleteById(Guid idTicketbook, Guid id)
        {
            await _ticketService.DeleteTicketById(idTicketbook, id);
        }
    }
}
