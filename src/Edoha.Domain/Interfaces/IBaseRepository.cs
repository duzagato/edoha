using System.Collections.Generic;
using System.Threading.Tasks;

namespace Edoha.Domain.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        Task<IEnumerable<T>> SelectAll();
        Task<T> SelectById(int id);
        Task Insert(T entity);
        Task Update(T entity);
        Task DeleteById(int id);

        Task<int> SelectCountById(int id);
        Task<T> ValidateId(int id);
    }
}