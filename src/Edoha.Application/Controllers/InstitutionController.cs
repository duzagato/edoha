using Microsoft.AspNetCore.Mvc;
using Edoha.Domain.Interfaces.Services;
using System.Net.Sockets;
using Edoha.Domain.Models.DTOs.Institution;

namespace Edoha.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InstitutionController : ControllerBase
    {
        private readonly IInstitutionService _institutionService;
        

        public InstitutionController(IInstitutionService institutionService)
        {
            _institutionService = institutionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var institutions = await _institutionService.SelectAllInstitutions();

                if (!institutions.Any())
                {
                    return NoContent();
                }

                return Ok(institutions);
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
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var institution = await _institutionService.SelectInstitutionById(id);

                if (institution == null)
                {
                    return NoContent();
                }

                return Ok(institution);
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
        public async Task<IActionResult> Create([FromBody] CreateInstitutionDTO request)
        {
            if(request != null)
            {
                try
                {
                    await _institutionService.InsertInstitution(request);
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
        public async Task<IActionResult> Update([FromBody] UpdateInstitutionDTO request)
        {
            if (request != null)
            {
                try
                {
                    await _institutionService.UpdateInstitutionById(request);
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
            await _institutionService.DeleteInstitutionById(id);
        }
    }
}
