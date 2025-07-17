using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edoha.Domain.Entities;
using Edoha.Domain.Models.DTOs.Institution;

namespace Edoha.Domain.Interfaces.Services
{
    public interface IInstitutionService : IService<Institution>
    {
        public Task InsertInstitution(CreateInstitutionDTO dto);

        public Task<Institution> SelectInstitutionById(Guid id);

        public Task<IEnumerable<Institution>> SelectAllInstitutions();

        public Task UpdateInstitutionById(UpdateInstitutionDTO dto);

        public Task DeleteInstitutionById(Guid id);
    }
}
