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

        public LotteryController(ILotteryService lotteryService)
        {
            _lotteryService = lotteryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid idInstitution)
        {
            try
            {
                var lotteries = await _lotteryService.SelectAllLotteries(idInstitution);

                if (!lotteries.Any())
                {
                    return NoContent();
                }

                return Ok(lotteries);
            }catch(Exception ex)
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
        public async Task<IActionResult> GetById(Guid idInstitution, Guid id)
        {
            try
            {
                var lottery = await _lotteryService.SelectLotteryById(idInstitution, id);

                if (lottery == null)
                {
                    return NoContent();
                }

                return Ok(lottery);
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
        public async Task<IActionResult> Create(Guid idInstitution, [FromBody] CreateLotteryDTO request)
        {
            if(request != null)
            {
                try
                {
                    await _lotteryService.InsertLottery(idInstitution, request);
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
        public async Task<IActionResult> Update(Guid idInstitution, [FromBody] UpdateLotteryDTO request)
        {
            if (request != null)
            {
                try
                {
                    await _lotteryService.UpdateLotteryById(idInstitution, request);
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
        public async Task DeleteById(Guid idInstitution, Guid id)
        {
            await _lotteryService.DeleteLotteryById(idInstitution, id);
        }
    }
}
