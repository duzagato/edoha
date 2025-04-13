using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Edoha.Domain.DTOs;

namespace Edoha.Domain.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        IDbConnection GetConnection();
        Task<IEnumerable<T>> SelectAll();
        Task<T> SelectById(int id);
        Task Insert(DTO dto);
        Task Update(DTO dto);
        Task DeleteById(int id);

        Task<int> SelectCountById(int id);
        Task<T> ValidateId(int? id);
    }
}