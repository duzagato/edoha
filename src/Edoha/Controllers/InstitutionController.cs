using Microsoft.AspNetCore.Mvc;
using Edoha.Application.Services;
using Edoha.Domain.DTOs.Institution;
using Edoha.Domain.Entities;
using System.Net.Sockets;

namespace Edoha.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InstitutionController : ControllerBase
    {
        private readonly InstitutionService _institutionService;

        public InstitutionController(InstitutionService institutionService)
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
        public async Task<IActionResult> GetById(int id)
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
        public async Task<IActionResult> Create([FromBody] CreateInstitutionDTO dto)
        {
            if(dto != null)
            {
                try
                {
                    await _institutionService.InsertInstitution(dto);
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
        public async Task<IActionResult> Update([FromBody] UpdateInstitutionDTO dto)
        {
            if (dto != null)
            {
                try
                {
                    await _institutionService.UpdateInstitutionById(dto);
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
            await _institutionService.DeleteInstitutionById(id);
        }
    }
}
