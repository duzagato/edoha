using Edoha.Shared.Helpers;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Repositories;
using Edoha.Domain.Interfaces.Services;
using Edoha.Domain.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Edoha.Domain.Interfaces.Context;

namespace Edoha.Domain.Services
{
    public abstract class Service<T> : IService<T> where T : class
    {
        protected readonly IBaseRepository<T> _repository;
        protected readonly IRequestValidationContext _requestValidationContext;

        public Service(IBaseRepository<T> repository, IRequestValidationContext requestValidationContext)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _requestValidationContext = requestValidationContext;
        }

        public async Task Insert(DTO dto)
        {
            await _requestValidationContext.ValidateDTO(dto);
            await _repository.Insert(dto);
        }

        public async Task Update(DTO dto)
        {
            await _requestValidationContext.ValidateDTO(dto);
            await _repository.Update(dto);
        }

        public async Task DeleteById(Guid id)
        {
            await _repository.IdExists(id);
            await _repository.DeleteById(id);
        }
    }
}
