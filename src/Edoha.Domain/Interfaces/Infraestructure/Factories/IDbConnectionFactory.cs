using System.Data;

namespace Edoha.Domain.Interfaces.Infraestructure.Factories
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
