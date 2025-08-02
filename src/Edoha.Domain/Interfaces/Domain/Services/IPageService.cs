using Edoha.Domain.Entities;
using Edoha.Domain.Models.DTOs.Page;

namespace Edoha.Domain.Interfaces.Domain.Services
{
    public interface IPageService : IService<Page>
    {
        Task<IEnumerable<Page>> SelectAllPages();
        Task<Page?> SelectPageById(Guid id);
        Task InsertPage(CreatePage request);
        Task UpdatePageById(UpdatePage request);
        Task DeletePageById(Guid id);
    }
}
