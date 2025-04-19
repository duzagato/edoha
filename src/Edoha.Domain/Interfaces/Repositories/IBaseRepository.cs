using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Edoha.Domain.Models.Requests;

namespace Edoha.Domain.Interfaces.Repositories
{
    public interface IBaseRepository<T> where T : class
    {
        IDbConnection GetConnection();
        Task<IEnumerable<T>> SelectAll();
        Task<T> SelectById(int id);
        Task Insert(Request dto);
        Task Update(Request dto);
        Task DeleteById(int id);

        Task<int> SelectCountById(int id);
        Task<T> ValidateId(int? id);
    }
}