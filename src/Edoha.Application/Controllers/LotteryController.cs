using Microsoft.AspNetCore.Mvc;
using Edoha.Domain.Models.DTOs.Lottery;
using Edoha.Domain.Interfaces.Domain.Services;

namespace Edoha.Controllers
{
    [ApiController]
    [Route("institution/{idInstitution}/lottery")]
    public class LotteryController : ControllerBase
    {
        private readonly ILotteryService _lotteryService;
        private readonly ILogger<LotteryController> _logger;

        public LotteryController(ILotteryService lotteryService, ILogger<LotteryController> logger)
        {
            _lotteryService = lotteryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid idInstitution)
        {
            _logger.LogInformation("[LotteryController.GetAll] Iniciado. Params: idInstitution={idInstitution}", idInstitution);

            try
            {
                var lotteries = await _lotteryService.SelectAllLotteries(idInstitution);

                if (!lotteries.Any())
                {
                    _logger.LogInformation("[LotteryController.GetAll] Nenhuma rifa encontrada para a instituição {idInstitution}. Retornando 204.", idInstitution);
                    return NoContent();
                }

                _logger.LogInformation("[LotteryController.GetAll] Retornando {count} rifa(s) para a instituição {idInstitution}.", lotteries.Count(), idInstitution);
                return Ok(lotteries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[LotteryController.GetAll] Erro inesperado. idInstitution={idInstitution}", idInstitution);
                return StatusCode(500, new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid idInstitution, Guid id)
        {
            _logger.LogInformation("[LotteryController.GetById] Iniciado. Params: idInstitution={idInstitution}, id={id}", idInstitution, id);

            try
            {
                var lottery = await _lotteryService.SelectLotteryById(idInstitution, id);

                if (lottery == null)
                {
                    _logger.LogInformation("[LotteryController.GetById] Rifa {id} não encontrada. Retornando 204.", id);
                    return NoContent();
                }

                _logger.LogInformation("[LotteryController.GetById] Retornando rifa {@lottery}.", lottery);
                return Ok(lottery);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[LotteryController.GetById] Erro inesperado. idInstitution={idInstitution}, id={id}", idInstitution, id);
                return StatusCode(500, new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid idInstitution, [FromBody] CreateLotteryDTO request)
        {
            _logger.LogInformation("[LotteryController.Create] Iniciado. Params: idInstitution={idInstitution}, request={@request}", idInstitution, request);

            if (request != null)
            {
                try
                {
                    await _lotteryService.InsertLottery(idInstitution, request);

                    _logger.LogInformation("[LotteryController.Create] Rifa criada com sucesso para a instituição {idInstitution}. Retornando 200.", idInstitution);
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[LotteryController.Create] Erro inesperado. idInstitution={idInstitution}", idInstitution);
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
                _logger.LogWarning("[LotteryController.Create] Request nulo ou vazio recebido. Retornando 400.");
                return BadRequest("Dados incompletos ou não enviados");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(Guid idInstitution, [FromBody] UpdateLotteryDTO request)
        {
            _logger.LogInformation("[LotteryController.Update] Iniciado. Params: idInstitution={idInstitution}, request={@request}", idInstitution, request);

            if (request != null)
            {
                try
                {
                    await _lotteryService.UpdateLotteryById(idInstitution, request);

                    _logger.LogInformation("[LotteryController.Update] Rifa atualizada com sucesso. idInstitution={idInstitution}, id={id}. Retornando 200.", idInstitution, request.Id);
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[LotteryController.Update] Erro inesperado. idInstitution={idInstitution}", idInstitution);
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
                _logger.LogWarning("[LotteryController.Update] Request nulo ou vazio recebido. Retornando 400.");
                return BadRequest("Dados incompletos ou não enviados");
            }
        }

        [HttpDelete("{id}")]
        public async Task DeleteById(Guid idInstitution, Guid id)
        {
            _logger.LogInformation("[LotteryController.DeleteById] Iniciado. Params: idInstitution={idInstitution}, id={id}", idInstitution, id);

            await _lotteryService.DeleteLotteryById(idInstitution, id);

            _logger.LogInformation("[LotteryController.DeleteById] Rifa {id} deletada com sucesso.", id);
        }
    }
}
