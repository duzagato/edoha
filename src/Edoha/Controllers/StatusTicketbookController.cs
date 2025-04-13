using Microsoft.AspNetCore.Mvc;
using Edoha.Application.Services;
using Edoha.Domain.DTOs.StatusTicketbook;
using Edoha.Domain.Entities;
using System.Net.Sockets;

namespace Edoha.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatusTicketbookController : ControllerBase
    {
        private readonly StatusTicketbookService _statusTicketbookService;

        public StatusTicketbookController(StatusTicketbookService statusTicketbookService)
        {
            _statusTicketbookService = statusTicketbookService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var statusTicketbooks = await _statusTicketbookService.SelectAllStatusTicketbooks();

                if (!statusTicketbooks.Any())
                {
                    return NoContent();
                }

                return Ok(statusTicketbooks);
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
                var statusTicketbook = await _statusTicketbookService.SelectStatusTicketbookById(id);

                if (statusTicketbook == null)
                {
                    return NoContent();
                }

                return Ok(statusTicketbook);
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
        public async Task<IActionResult> Create([FromBody] CreateStatusTicketbookDTO dto)
        {
            if(dto != null)
            {
                try
                {
                    await _statusTicketbookService.InsertStatusTicketbook(dto);
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
        public async Task<IActionResult> Update([FromBody] UpdateStatusTicketbookDTO dto)
        {
            if (dto != null)
            {
                try
                {
                    await _statusTicketbookService.UpdateStatusTicketbookById(dto);
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
            await _statusTicketbookService.DeleteStatusTicketbookById(id);
        }
    }
}
