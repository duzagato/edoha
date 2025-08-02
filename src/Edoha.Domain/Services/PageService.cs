using Edoha.Domain.Entities;
using Edoha.Domain.Models.DTOs.Page;
using Edoha.Domain.Interfaces.Infraestructure.Context;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Interfaces.Domain.Services;

namespace Edoha.Domain.Services
{
    public class PageService : Service<Page>, IPageService
    {
        public PageService(IPageRepository repository,
            IRequestValidationContext requestValidationContext)
            : base(repository, requestValidationContext) { }

        public async Task InsertPage(CreatePage dto)
        {
            await Insert(dto);
        }

        public async Task<Page> SelectPageById(Guid id)
        {
            return await _repository.SelectById(id);
        }

        public async Task<IEnumerable<Page>> SelectAllPages()
        {
            return await _repository.SelectAll();
        }

        public async Task UpdatePageById(UpdatePage dto)
        {
            await Update(dto);
        }

        public async Task DeletePageById(Guid id)
        {
            await DeleteById(id);
        }
    }
}
