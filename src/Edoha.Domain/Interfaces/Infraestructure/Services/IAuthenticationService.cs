using Edoha.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Interfaces.Infraestructure.Services
{
    public interface IAuthenticationService
    {
        string GenerateToken(User user, IEnumerable<string> permissions);
    }
}
