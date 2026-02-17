using Microsoft.AspNetCore.Mvc;
using Edoha.Domain.Models.DTOs.Ticketbook;
using Edoha.Domain.Interfaces.Domain.Services;
using Edoha.Domain.Models.Requests.Ticketbook;

namespace Edoha.Controllers
{
    [ApiController]
    [Route("ticketbook")]
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
        public async Task<IActionResult> GetReturnedTicketbooks([FromQuery] Guid idLottery)
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
        public async Task<IActionResult> GetWithdrawnsTicketbooks([FromQuery] Guid idLottery)
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
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var ticketbooks = await _ticketbookService.SelectAllTicketbooks();

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
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var ticketbook = await _ticketbookService.SelectTicketbookById(id);

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
        public async Task<IActionResult> PatchStatusTicketbook([FromRoute] Guid idTicketbook, int idStatusTicketbook)
        {
            try
            {
                await _ticketbookService.ChangeTicketbookStatus(idStatusTicketbook, idTicketbook);
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
        public async Task<IActionResult> PatchStatusTicketbook([FromRoute] Guid idTicketbook)
        {
            try
            {
                await _ticketbookService.ChangeTicketbookStatusToReturned(idTicketbook);
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
        public async Task<IActionResult> PatchStatusTicketbookToWithdraw([FromRoute] Guid idTicketbook)
        {
            try
            {
                await _ticketbookService.ChangeTicketbookStatusToWithdraw(idTicketbook);
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
        public async Task<IActionResult> Create([FromBody] CreateTicketbookDTO request)
        {
            if (request != null)
            {
                try
                {
                    await _ticketbookService.InsertTicketbook(request);
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
        public async Task<IActionResult> Update([FromBody] UpdateTicketbookDTO request)
        {
            if (request != null)
            {
                try
                {
                    await _ticketbookService.UpdateTicketbookById(request);
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
            await _ticketbookService.DeleteTicketbookById(id);
        }
    }
}
