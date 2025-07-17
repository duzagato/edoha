using System;
using System.Data;

namespace Edoha.Domain.Interfaces.Factory
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
