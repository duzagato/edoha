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
            _logger.LogInformation("Iniciando método GetReturnedTicketbooks");
            _logger.LogInformation("Parâmetros recebidos - idLottery: {IdLottery}", idLottery);
            
            try
            {
                var ticketbooks = await _ticketbookService.SelectReturnedsTicketbooks(idLottery);

                if (!ticketbooks.Any())
                {
                    _logger.LogInformation("Nenhum talão devolvido encontrado para a rifa {IdLottery}. Retornando NoContent", idLottery);
                    return NoContent();
                }

                _logger.LogInformation("Retornando {Count} talões devolvidos para a rifa {IdLottery}", ticketbooks.Count(), idLottery);
                return Ok(ticketbooks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar talões devolvidos para a rifa {IdLottery}", idLottery);
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
            _logger.LogInformation("Iniciando método GetWithdrawnsTicketbooks");
            _logger.LogInformation("Parâmetros recebidos - idLottery: {IdLottery}", idLottery);
            
            try
            {
                var ticketbooks = await _ticketbookService.SelectWithdrawnsTicketbooks(idLottery);

                if (!ticketbooks.Any())
                {
                    _logger.LogInformation("Nenhum talão retirado encontrado para a rifa {IdLottery}. Retornando NoContent", idLottery);
                    return NoContent();
                }

                _logger.LogInformation("Retornando {Count} talões retirados para a rifa {IdLottery}", ticketbooks.Count(), idLottery);
                return Ok(ticketbooks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar talões retirados para a rifa {IdLottery}", idLottery);
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
            _logger.LogInformation("Iniciando método GetAll");
            _logger.LogInformation("Parâmetros recebidos - idLottery: {IdLottery}", idLottery);
            
            try
            {
                var ticketbooks = await _ticketbookService.SelectAllTicketbooks(idLottery);

                if (!ticketbooks.Any())
                {
                    _logger.LogInformation("Nenhum talão encontrado para a rifa {IdLottery}. Retornando NoContent", idLottery);
                    return NoContent();
                }

                _logger.LogInformation("Retornando {Count} talões para a rifa {IdLottery}", ticketbooks.Count(), idLottery);
                return Ok(ticketbooks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todos os talões para a rifa {IdLottery}", idLottery);
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
            _logger.LogInformation("Iniciando método GetById");
            _logger.LogInformation("Parâmetros recebidos - idLottery: {IdLottery}, id: {Id}", idLottery, id);
            
            try
            {
                var ticketbook = await _ticketbookService.SelectTicketbookById(id, idLottery);

                if (ticketbook == null)
                {
                    _logger.LogInformation("Talão {Id} não encontrado para a rifa {IdLottery}. Retornando NoContent", id, idLottery);
                    return NoContent();
                }

                _logger.LogInformation("Retornando talão {Id} da rifa {IdLottery}", id, idLottery);
                return Ok(ticketbook);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar talão {Id} da rifa {IdLottery}", id, idLottery);
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
            _logger.LogInformation("Iniciando método GetTicketbookByNumber");
            _logger.LogInformation("Parâmetros recebidos - idLottery: {IdLottery}, numberTicketbook: {NumberTicketbook}", idLottery, numberTicketbook);
            
            if (idLottery == Guid.Empty || numberTicketbook <= 0)
            {
                _logger.LogWarning("Dados incompletos ou inválidos - idLottery: {IdLottery}, numberTicketbook: {NumberTicketbook}", idLottery, numberTicketbook);
                return BadRequest("Dados incompletos ou não enviados");
            }

            try
            {
                var ticketbook = await _ticketbookService.GetTicketbookByNumber(idLottery, numberTicketbook);

                if (ticketbook == null)
                {
                    _logger.LogInformation("Talão número {NumberTicketbook} não encontrado para a rifa {IdLottery}. Retornando NoContent", numberTicketbook, idLottery);
                    return NoContent();
                }

                _logger.LogInformation("Retornando talão número {NumberTicketbook} da rifa {IdLottery}", numberTicketbook, idLottery);
                return Ok(ticketbook);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar talão número {NumberTicketbook} da rifa {IdLottery}", numberTicketbook, idLottery);
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
            _logger.LogInformation("Iniciando método PatchStatusTicketbook");
            _logger.LogInformation("Parâmetros recebidos - idLottery: {IdLottery}, idTicketbook: {IdTicketbook}, idStatusTicketbook: {IdStatusTicketbook}", idLottery, idTicketbook, idStatusTicketbook);
            
            try
            {
                await _ticketbookService.ChangeTicketbookStatus(idStatusTicketbook, idTicketbook, idLottery);
                _logger.LogInformation("Alteração de status do talão {IdTicketbook} executada com sucesso!", idTicketbook);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar status do talão {IdTicketbook} para {IdStatusTicketbook}", idTicketbook, idStatusTicketbook);
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
            _logger.LogInformation("Iniciando método PatchStatusTicketbook (returned)");
            _logger.LogInformation("Parâmetros recebidos - idLottery: {IdLottery}, idTicketbook: {IdTicketbook}", idLottery, idTicketbook);
            
            try
            {
                await _ticketbookService.ChangeTicketbookStatusToReturned(idTicketbook, idLottery);
                _logger.LogInformation("Alteração de status do talão {IdTicketbook} para devolvido executada com sucesso!", idTicketbook);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar status do talão {IdTicketbook} para devolvido", idTicketbook);
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
            _logger.LogInformation("Iniciando método PatchStatusTicketbookToWithdraw");
            _logger.LogInformation("Parâmetros recebidos - idLottery: {IdLottery}, idTicketbook: {IdTicketbook}", idLottery, idTicketbook);
            
            try
            {
                await _ticketbookService.ChangeTicketbookStatusToWithdraw(idTicketbook, idLottery);
                _logger.LogInformation("Alteração de status do talão {IdTicketbook} para retirado executada com sucesso!", idTicketbook);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar status do talão {IdTicketbook} para retirado", idTicketbook);
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
            _logger.LogInformation("Iniciando método Create");
            _logger.LogInformation("Parâmetros recebidos - idLottery: {IdLottery}, request: {@Request}", idLottery, request);
            
            if (request != null)
            {
                try
                {
                    Guid id = await _ticketbookService.InsertTicketbook(request, idLottery);
                    
                    _logger.LogInformation("Talão criado com sucesso. IdTicketbook: {IdTicketbook}", id);
                    
                    return Created(string.Empty, new
                    {
                        IdTicketbook = id
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao criar talão para a rifa {IdLottery}", idLottery);
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
                _logger.LogWarning("Tentativa de criar talão com dados nulos ou incompletos");
                return BadRequest("Dados incompletos ou não enviados");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromRoute] Guid idLottery, [FromBody] UpdateTicketbookDTO request)
        {
            _logger.LogInformation("Iniciando método Update");
            _logger.LogInformation("Parâmetros recebidos - idLottery: {IdLottery}, request: {@Request}", idLottery, request);
            
            if (request != null)
            {
                try
                {
                    await _ticketbookService.UpdateTicketbookById(request, idLottery);
                    
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao atualizar talão para a rifa {IdLottery}", idLottery);
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
                _logger.LogWarning("Tentativa de atualizar talão com dados nulos ou incompletos");
                return BadRequest("Dados incompletos ou não enviados");
            }
        }

        [HttpDelete("{id}")]
        public async Task DeleteById([FromRoute] Guid idLottery, Guid id)
        {
            _logger.LogInformation("Iniciando método DeleteById");
            _logger.LogInformation("Parâmetros recebidos - idLottery: {IdLottery}, id: {Id}", idLottery, id);
            
            await _ticketbookService.DeleteTicketbookById(id, idLottery);
            
            _logger.LogInformation("Talão {Id} deletado com sucesso da rifa {IdLottery}", id, idLottery);
        }
    }
}
