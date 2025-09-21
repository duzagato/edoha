using Edoha.Domain.Entities;

namespace Edoha.Domain.Interfaces.Infraestructure.Repositories
{
    public interface ITableConfigurationRepository : IBaseRepository<TableConfiguration>
    {
        Task<IEnumerable<TableConfiguration>?> SelectAllConfigurationsByTableName(string schema, string tableName);
    }
}
