using Dapper;
using Edoha.Domain.Entities;
using Edoha.Domain.Helpers;
using Edoha.Domain.Interfaces.Infraestructure.Factories;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Models.DTOs;
using Edoha.Domain.Models.Login;
using Edoha.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Infraestructure.Repositories
{
    public class LoginRepository : BaseRepository<Login>, ILoginRepository
    {
        public LoginRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory) { }
    }
}
