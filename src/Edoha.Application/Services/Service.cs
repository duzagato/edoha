using Edoha.Application.Helpers;
using Edoha.Domain.DTOs;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Application.Services
{
    public abstract class Service<T> where T : class
    {
        protected readonly IBaseRepository<T> _repository;

        public Service(IBaseRepository<T> repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task Insert(DTO dto)
        {
            dto.Validate();
            await _repository.Insert(dto);
        }

        public async Task Update(DTO dto)
        {
            dto.Validate();
            await _repository.Update(dto);
        }

        public async Task DeleteById(int id)
        {
            await _repository.ValidateId(id);
            await _repository.DeleteById(id);
        }
    }
}
