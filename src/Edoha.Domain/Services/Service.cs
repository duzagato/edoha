using Edoha.Shared.Helpers;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Repositories;
using Edoha.Domain.Interfaces.Services;
using Edoha.Domain.Models.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Services
{
    public abstract class Service<T> : IService<T> where T : class
    {
        protected readonly IBaseRepository<T> _repository;

        public Service(IBaseRepository<T> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task Insert(Request request)
        {
            request.Validate();
            await _repository.Insert(request);
        }

        public async Task Update(Request request)
        {
            request.Validate();
            await _repository.Update(request);
        }

        public async Task DeleteById(int id)
        {
            await _repository.ValidateId(id);
            await _repository.DeleteById(id);
        }
    }
}
