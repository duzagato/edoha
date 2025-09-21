using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Interfaces.Infraestructure.Services;

namespace Edoha.Infraestructure.Services
{
    public class TableConfigurationService : ITableConfigurationService
    {
        private readonly ITableConfigurationRepository _tableConfigurationRepository;
        public TableConfigurationService(ITableConfigurationRepository tableConfigurationRepository) 
        {
            _tableConfigurationRepository = tableConfigurationRepository;
        }

        public Task<IEnumerable<TableConfiguration>?> GetAllTableConfigurations(string schema, string tableName)
        {
            var configurations = _tableConfigurationRepository.SelectAllConfigurationsByTableName(schema, tableName);

            return configurations;
        }
    }
}
