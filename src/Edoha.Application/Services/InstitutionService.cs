using Edoha.Application.Helpers;
using Edoha.Domain.DTOs.Institution;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Application.Services
{
    public class InstitutionService : Service<Institution>
    {

        public InstitutionService(IInstitutionRepository repository) : base(repository) { }

        public async Task InsertInstitution(CreateInstitutionDTO dto)
        {
            await this.Insert(dto);
        }

        public async Task<Institution> SelectInstitutionById(int id)
        {
            return await _repository.SelectById(id);
        }

        public async Task<IEnumerable<Institution>> SelectAllInstitutions()
        {
            return await _repository.SelectAll();
        }

        public async Task UpdateInstitutionById(UpdateInstitutionDTO dto)
        {
            await this.Update(dto);
        }

        public async Task DeleteInstitutionById(int id)
        {
            await this.DeleteById(id);
        }
    }
}
