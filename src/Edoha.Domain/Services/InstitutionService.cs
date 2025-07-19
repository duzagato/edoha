using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Context;
using Edoha.Domain.Interfaces.Repositories;
using Edoha.Domain.Interfaces.Services;
using Edoha.Domain.Models.DTOs.Institution;
using Edoha.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Services
{
    public class InstitutionService : Service<Institution>, IInstitutionService
    {

        public InstitutionService(IInstitutionRepository repository, 
            IRequestValidationContext requestValidationContext
            ) : base(repository, requestValidationContext) { }

        public async Task InsertInstitution(CreateInstitutionDTO dto)
        {
            await Insert(dto);
        }

        public async Task<Institution> SelectInstitutionById(Guid id)
        {
            return await _repository.SelectById(id);
        }

        public async Task<IEnumerable<Institution>> SelectAllInstitutions()
        {
            return await _repository.SelectAll();
        }

        public async Task UpdateInstitutionById(UpdateInstitutionDTO dto)
        {
            await Update(dto);
        }

        public async Task DeleteInstitutionById(Guid id)
        {
            await DeleteById(id);
        }
    }
}
