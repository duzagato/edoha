using Microsoft.AspNetCore.Mvc;
using Edoha.Domain.Models.DTOs.Institution;
using Edoha.Domain.Interfaces.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Edoha.Controllers
{
    [ApiController]
    [Route("institution")]
    public class InstitutionController : ControllerBase
    {
        private readonly IInstitutionService _institutionService;
        private readonly ILogger<InstitutionController> _logger;
        

        public InstitutionController(IInstitutionService institutionService, ILogger<InstitutionController> logger)
        {
            _institutionService = institutionService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Iniciando método GetAll (InstitutionController)");
            
            try
            {
                _logger.LogInformation("Chamando _institutionService.SelectAllInstitutions()");
                var institutions = await _institutionService.SelectAllInstitutions();

                if (!institutions.Any())
                {
                    _logger.LogInformation("Nenhuma instituição encontrada. Retornando NoContent");
                    return NoContent();
                }

                _logger.LogInformation("Retornando {Count} instituições encontradas", institutions.Count());
                return Ok(institutions);
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar GetAll: {Message}", ex.Message);
                return StatusCode(500, new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message
                });
            }
        }

        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            _logger.LogInformation("Iniciando método GetBySlug (InstitutionController)");
            _logger.LogInformation("Parâmetros recebidos - slug: {Slug}", slug);
            
            try
            {
                _logger.LogInformation("Chamando _institutionService.SelectInstitutionBySlug com slug: {Slug}", slug);
                var institution = await _institutionService.SelectInstitutionBySlug(slug);

                if (institution == null)
                {
                    _logger.LogInformation("Instituição com slug {Slug} não encontrada. Retornando NoContent", slug);
                    return NoContent();
                }

                _logger.LogInformation("Instituição com slug {Slug} encontrada. Retornando dados da instituição", slug);
                return Ok(institution);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar GetBySlug com slug {Slug}: {Message}", slug, ex.Message);
                return StatusCode(500, new
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message
                });
            }
        }



        [HttpGet("institution_by_user/{idUser}")]
        public async Task<IActionResult> GetByUser(Guid idUser)
        {
            _logger.LogInformation("Iniciando método GetByUser (InstitutionController)");
            _logger.LogInformation("Parâmetros recebidos - idUser: {IdUser}", idUser);
            
            try
            {
                _logger.LogInformation("Chamando _institutionService.SelectInstitutionsByUser com idUser: {IdUser}", idUser);
                var institutions = await _institutionService.SelectInstitutionsByUser(idUser);

                if (!institutions.Any())
                {
                    _logger.LogInformation("Nenhuma instituição encontrada para o usuário {IdUser}. Retornando NoContent", idUser);
                    return NoContent();
                }

                _logger.LogInformation("Retornando {Count} instituições para o usuário {IdUser}", institutions.Count(), idUser);
                return Ok(institutions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao executar GetByUser com idUser {IdUser}: {Message}", idUser, ex.Message);
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
            _logger.LogInformation("Iniciando método Create (InstitutionController)");
            _logger.LogInformation("Parâmetros recebidos - request: {@Request}", request);
            
            if(request != null)
            {
                try
                {
                    _logger.LogInformation("Chamando _institutionService.InsertInstitution");
                    await _institutionService.InsertInstitution(request);
                    
                    _logger.LogInformation("Instituição criada com sucesso. Retornando Ok");
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao executar Create: {Message}", ex.Message);
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
                _logger.LogWarning("Dados incompletos ou não enviados no método Create");
                return BadRequest("Dados incompletos ou não enviados");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateInstitutionDTO request)
        {
            _logger.LogInformation("Iniciando método Update (InstitutionController)");
            _logger.LogInformation("Parâmetros recebidos - request: {@Request}", request);
            
            if (request != null)
            {
                try
                {
                    _logger.LogInformation("Chamando _institutionService.UpdateInstitutionById");
                    await _institutionService.UpdateInstitutionById(request);
                    
                    _logger.LogInformation("Instituição atualizada com sucesso. Retornando Ok");
                    return Ok();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao executar Update: {Message}", ex.Message);
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
                _logger.LogWarning("Dados incompletos ou não enviados no método Update");
                return BadRequest("Dados incompletos ou não enviados");
            }
        }

        [HttpDelete("{id}")]
        public async Task DeleteById(Guid id)
        {
            _logger.LogInformation("Iniciando método DeleteById (InstitutionController)");
            _logger.LogInformation("Parâmetros recebidos - id: {Id}", id);
            
            _logger.LogInformation("Chamando _institutionService.DeleteInstitutionById com id: {Id}", id);
            await _institutionService.DeleteInstitutionById(id);
            
            _logger.LogInformation("Instituição com id {Id} deletada com sucesso", id);
        }
    }
}
