using Edoha.Domain.Models.DTOs;

namespace Edoha.Domain.Interfaces.Domain.Services
{
    public interface IService<T> where T : class
    {
        public Task Insert(DTO dto);

        public Task Update(DTO dto);

        public Task DeleteById(Guid id);
    }
}
