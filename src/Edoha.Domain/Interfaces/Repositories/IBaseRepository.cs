using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Edoha.Domain.Models.DTOs;

namespace Edoha.Domain.Interfaces.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        Task<IEnumerable<T>> SelectAll();

        Task<T> SelectById(Guid? id);

        Task Insert(DTO dto);

        Task Update(DTO dto);

        Task DeleteById(Guid id);

        Task<int> SelectCountById(Guid id);

        Task IdExists(Guid? id);
    }
}