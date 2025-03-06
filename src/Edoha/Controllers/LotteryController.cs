using Microsoft.AspNetCore.Mvc;
using Edoha.Domain.DTOs.Lottery;
using Edoha.Domain.Services;
using System.Net.Sockets;

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



        [HttpPost]
        public IActionResult Create([FromBody] CreateLotteryDTO dto)
        {
            if(dto != null)
            {
                try
                {
                    _lotteryService.CreateLottery(dto);
                    return Ok("Loteria criada com sucesso!");
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
    }
}
