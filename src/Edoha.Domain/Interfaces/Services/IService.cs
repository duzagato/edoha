using Edoha.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edoha.Domain.Models.DTOs;

namespace Edoha.Domain.Interfaces.Services
{
    public interface IService<T> where T : class
    {
        public Task Insert(DTO dto);

        public Task Update(DTO dto);

        public Task DeleteById(Guid id);
    }
}
