using Microsoft.AspNetCore.Mvc;
using Edoha.Domain.Interfaces.Services;
using Edoha.Domain.Entities;
using System.Net.Sockets;
using Edoha.Domain.Models.Requests.Lottery;

namespace Edoha.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LotteryController : ControllerBase
    {
        private readonly ILotteryService _lotteryService;

        public LotteryController(ILotteryService lotteryService)
        {
            _lotteryService = lotteryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var lotteries = await _lotteryService.SelectAllLotteries();

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
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var lottery = await _lotteryService.SelectLotteryById(id);

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
        public async Task<IActionResult> Create([FromBody] CreateLotteryRequest request)
        {
            if(request != null)
            {
                try
                {
                    await _lotteryService.InsertLottery(request);
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
        public async Task<IActionResult> Update([FromBody] UpdateLotteryRequest request)
        {
            if (request != null)
            {
                try
                {
                    await _lotteryService.UpdateLotteryById(request);
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
        public async Task DeleteById(int id)
        {
            await _lotteryService.DeleteLotteryById(id);
        }
    }
}
