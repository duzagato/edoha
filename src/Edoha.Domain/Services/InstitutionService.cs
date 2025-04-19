using Edoha.Shared.Helpers;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Repositories;
using Edoha.Domain.Interfaces.Services;
using Edoha.Domain.Models.Requests.Institution;
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

        public InstitutionService(IInstitutionRepository repository) : base(repository) { }

        public async Task InsertInstitution(CreateInstitutionRequest request)
        {
            await this.Insert(request);
        }

        public async Task<Institution> SelectInstitutionById(int id)
        {
            return await _repository.SelectById(id);
        }

        public async Task<IEnumerable<Institution>> SelectAllInstitutions()
        {
            return await _repository.SelectAll();
        }

        public async Task UpdateInstitutionById(UpdateInstitutionRequest request)
        {
            await this.Update(request);
        }

        public async Task DeleteInstitutionById(int id)
        {
            await this.DeleteById(id);
        }
    }
}
