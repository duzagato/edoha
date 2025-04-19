using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edoha.Domain.Entities;
using Edoha.Domain.Models.Requests.Institution;

namespace Edoha.Domain.Interfaces.Services
{
    public interface IInstitutionService : IService<Institution>
    {
        public Task InsertInstitution(CreateInstitutionRequest request);

        public Task<Institution> SelectInstitutionById(int id);

        public Task<IEnumerable<Institution>> SelectAllInstitutions();

        public Task UpdateInstitutionById(UpdateInstitutionRequest request);

        public Task DeleteInstitutionById(int id);
    }
}
