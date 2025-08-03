using System;
using System.Data;

namespace Edoha.Domain.Interfaces.Infraestructure.Context
{
    public interface IDbConnectionContext
    {
        IDbConnection CreateConnection();
    }
}
