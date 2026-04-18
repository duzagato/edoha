using Microsoft.AspNetCore.Mvc;
using Edoha.Domain.Models.DTOs.Ticketbook;
using Edoha.Domain.Interfaces.Domain.Services;
using Edoha.Domain.Models.Requests.Ticketbook;

namespace Edoha.Controllers
{
    [ApiController]
    [Route("lottery/{idLottery:guid}/ticketbook")]
    public class TicketbookController : ControllerBase
    {
        private readonly ITicketbookService _ticketbookService;
        private readonly ILogger<TicketbookController> _logger;

        public TicketbookController(ITicketbookService ticketbookService, ILogger<TicketbookController> logger)
        {
            _ticketbookService = ticketbookService;
            _logger = logger;
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet("returneds")]
        public async Task<IActionResult> GetReturnedTicketbooks([FromRoute] Guid idLottery)
        {
            try
            {
                var ticketbooks = await _ticketbookService.SelectReturnedsTicketbooks(idLottery);

                if (!ticketbooks.Any())
                {
                    return NoContent();
                }

                return Ok(ticketbooks);
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

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet("withdrawns")]
        public async Task<IActionResult> GetWithdrawnsTicketbooks([FromRoute] Guid idLottery)
        {
            try
            {
                var ticketbooks = await _ticketbookService.SelectWithdrawnsTicketbooks(idLottery);

                if (!ticketbooks.Any())
                {
                    return NoContent();
                }

                return Ok(ticketbooks);
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

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid idLottery)
        {
            try
            {
                var ticketbooks = await _ticketbookService.SelectAllTicketbooks(idLottery);

                if (!ticketbooks.Any())
                {
                    return NoContent();
                }

                return Ok(ticketbooks);
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid idLottery, Guid id)
        {
            try
            {
                var ticketbook = await _ticketbookService.SelectTicketbookById(id, idLottery);

                if (ticketbook == null)
                {
                    return NoContent();
                }

                return Ok(ticketbook);
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

        [HttpGet("ticketbook_by_number/{numberTicketbook}")]
        public async Task<IActionResult> GetTicketbookByNumber(Guid idLottery, int numberTicketbook)
        {
            if (idLottery == Guid.Empty || numberTicketbook <= 0)
            {
                return BadRequest("Dados incompletos ou não enviados");
            }

            try
            {
                var ticketbook = await _ticketbookService.GetTicketbookByNumber(idLottery, numberTicketbook);

                if (ticketbook == null)
                {
                    return NoContent();
                }

                return Ok(ticketbook);
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

        [HttpPatch("{idTicketbook}/status/{idStatusTicketbook}")]
        public async Task<IActionResult> PatchStatusTicketbook([FromRoute] Guid idLottery, [FromRoute] Guid idTicketbook, int idStatusTicketbook)
        {
            try
            {
                await _ticketbookService.ChangeTicketbookStatus(idStatusTicketbook, idTicketbook, idLottery);
                _logger.LogInformation("Alteração executada com sucesso!");

                return NoContent();
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

        [HttpPatch("{idTicketbook}/status/returned")]
        public async Task<IActionResult> PatchStatusTicketbook([FromRoute] Guid idLottery, [FromRoute] Guid idTicketbook)
        {
            try
            {
                await _ticketbookService.ChangeTicketbookStatusToReturned(idTicketbook, idLottery);
                _logger.LogInformation("Alteração executada com sucesso!");

                return NoContent();
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

        [HttpPatch("{idTicketbook}/status/withdraw")]
        public async Task<IActionResult> PatchStatusTicketbookToWithdraw([FromRoute] Guid idLottery, [FromRoute] Guid idTicketbook)
        {
            try
            {
                await _ticketbookService.ChangeTicketbookStatusToWithdraw(idTicketbook, idLottery);
                _logger.LogInformation("Alteração executada com sucesso!");

                return NoContent();
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
        public async Task<IActionResult> Create([FromRoute] Guid idLottery, [FromBody] PostTicketbookRequest request)
        {
            if (request != null)
            {
                try
                {
                    Guid id = await _ticketbookService.InsertTicketbook(request, idLottery);
                    return Created(string.Empty, new
                    {
                        IdTicketbook = id
                    });
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
        public async Task<IActionResult> Update([FromRoute] Guid idLottery, [FromBody] UpdateTicketbookDTO request)
        {
            if (request != null)
            {
                try
                {
                    await _ticketbookService.UpdateTicketbookById(request, idLottery);
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
        public async Task DeleteById([FromRoute] Guid idLottery, Guid id)
        {
            await _ticketbookService.DeleteTicketbookById(id, idLottery);
        }
    }
}
