using Edoha.Domain.Entities;
using Edoha.Domain.Models.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Interfaces.Infraestructure.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> SelectUserCredentialsByNickname(string nickname);
    }
}
