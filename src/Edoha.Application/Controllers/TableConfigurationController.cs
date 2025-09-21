using Edoha.Domain.Interfaces.Infraestructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Edoha.Application.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TableConfigurationController : ControllerBase
    {
        private readonly ITableConfigurationService _tableConfigurationService;
        public TableConfigurationController(ITableConfigurationService tableConfigurationService) 
        {
            _tableConfigurationService = tableConfigurationService;
        }

        [HttpGet("{schema}/{tableName}")]
        public async Task<IActionResult> GetAll(string schema, string tableName)
        {
            var configurations = await _tableConfigurationService.GetAllTableConfigurations(schema, tableName);

            if (configurations is not null)
            {
                return Ok(configurations);
            }
            else
            {
                return NoContent();
            }
        }
    }
}
