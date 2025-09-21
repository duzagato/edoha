using Edoha.Domain.Entities;

namespace Edoha.Domain.Interfaces.Infraestructure.Services
{
    public interface ITableConfigurationService
    {
        Task<IEnumerable<TableConfiguration>?> GetAllTableConfigurations(string schema, string tableName);
    }
}
