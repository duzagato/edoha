using Microsoft.AspNetCore.Mvc;
using Edoha.Application.Services;
using Edoha.Domain.DTOs.Ticketbook;
using Edoha.Domain.Entities;
using System.Net.Sockets;

namespace Edoha.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TicketbookController : ControllerBase
    {
        private readonly TicketbookService _ticketbookService;

        public TicketbookController(TicketbookService ticketbookService)
        {
            _ticketbookService = ticketbookService;
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
        public async Task<IActionResult> GetById(int id)
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



        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTicketbookDTO dto)
        {
            if (dto != null)
            {
                try
                {
                    await _ticketbookService.InsertTicketbook(dto);
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
        public async Task<IActionResult> Update([FromBody] UpdateTicketbookDTO dto)
        {
            if (dto != null)
            {
                try
                {
                    await _ticketbookService.UpdateTicketbookById(dto);
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
            await _ticketbookService.DeleteTicketbookById(id);
        }
    }
}
