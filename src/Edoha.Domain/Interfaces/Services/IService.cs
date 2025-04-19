using Edoha.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edoha.Domain.Models.Requests;

namespace Edoha.Domain.Interfaces.Services
{
    public interface IService<T> where T : class
    {
        public Task Insert(Request request);

        public Task Update(Request request);

        public Task DeleteById(int id);
    }
}
