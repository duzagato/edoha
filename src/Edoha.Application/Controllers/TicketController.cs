using Microsoft.AspNetCore.Mvc;
using Edoha.Domain.Models.DTOs.Ticket;
using Edoha.Domain.Interfaces.Domain.Services;
using Edoha.Domain.Models.Requests.Ticket;
using Microsoft.Extensions.Logging;

namespace Edoha.Controllers
{
    [ApiController]
    [Route("ticketbook/{idTicketbook}/ticket")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly ILogger<TicketController> _logger;

        public TicketController(ITicketService ticketService, ILogger<TicketController> logger)
        {
            _ticketService = ticketService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid idTicketbook)
        {
            _logger.LogInformation("Iniciando método GetAll");
            _logger.LogInformation("Parâmetros recebidos - idTicketbook: {IdTicketbook}", idTicketbook);

            var tickets = await _ticketService.SelectAllTickets(idTicketbook);

            if (!tickets.Any())
            {
                _logger.LogInformation("Nenhum ticket encontrado para o ticketbook {IdTicketbook}. Retornando NoContent", idTicketbook);
                return NoContent();
            }

            _logger.LogInformation("Total de {Count} tickets encontrados para o ticketbook {IdTicketbook}. Retornando Ok com os dados", tickets.Count(), idTicketbook);
            return Ok(tickets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid idTicketbook, Guid id)
        {
            _logger.LogInformation("Iniciando método GetById");
            _logger.LogInformation("Parâmetros recebidos - idTicketbook: {IdTicketbook}, id: {Id}", idTicketbook, id);

            var ticket = await _ticketService.SelectTicketById(idTicketbook, id);

            if (ticket == null)
            {
                _logger.LogInformation("Ticket com id {Id} não encontrado no ticketbook {IdTicketbook}. Retornando NoContent", id, idTicketbook);
                return NoContent();
            }

            _logger.LogInformation("Ticket com id {Id} encontrado no ticketbook {IdTicketbook}. Retornando Ok com os dados: {@Ticket}", id, idTicketbook, ticket);
            return Ok(ticket);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid idTicketbook, [FromBody] List<CreateTicketRequest> request)
        {
            _logger.LogInformation("Iniciando método Create");
            _logger.LogInformation("Parâmetros recebidos - idTicketbook: {IdTicketbook}, request: {@Request}", idTicketbook, request);

            if (request != null && request.Count > 0)
            {
                _logger.LogInformation("Processando {Count} tickets para criação", request.Count);
                
                foreach(var ticket in request)
                {
                    _logger.LogInformation("Criando ticket com número: {Number}", ticket.Number);
                    await _ticketService.InsertTicket(idTicketbook, ticket);
                }

                _logger.LogInformation("Todos os {Count} tickets foram criados com sucesso. Retornando Created", request.Count);
                return Created();
            }
            else
            {
                _logger.LogWarning("Request inválido ou vazio recebido. Retornando BadRequest");
                return BadRequest("Dados incompletos ou não enviados");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(Guid idTicketbook, [FromBody] UpdateTicketDTO request)
        {
            _logger.LogInformation("Iniciando método Update");
            _logger.LogInformation("Parâmetros recebidos - idTicketbook: {IdTicketbook}, request: {@Request}", idTicketbook, request);

            if (request != null)
            {
                await _ticketService.UpdateTicketById(idTicketbook, request);
                _logger.LogInformation("Ticket atualizado com sucesso no ticketbook {IdTicketbook}. Retornando Ok", idTicketbook);
                return Ok();
            }
            else
            {
                _logger.LogWarning("Request inválido ou vazio recebido. Retornando BadRequest");
                return BadRequest("Dados incompletos ou não enviados");
            }
        }

        [HttpDelete("{id}")]
        public async Task DeleteById(Guid idTicketbook, Guid id)
        {
            _logger.LogInformation("Iniciando método DeleteById");
            _logger.LogInformation("Parâmetros recebidos - idTicketbook: {IdTicketbook}, id: {Id}", idTicketbook, id);

            await _ticketService.DeleteTicketById(idTicketbook, id);
            
            _logger.LogInformation("Ticket com id {Id} deletado com sucesso do ticketbook {IdTicketbook}", id, idTicketbook);
        }
    }
}
