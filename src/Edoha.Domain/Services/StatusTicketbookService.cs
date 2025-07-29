using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Domain.Services;
using Edoha.Domain.Interfaces.Infraestructure.Context;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Models.DTOs.StatusTicketbook;

namespace Edoha.Domain.Services
{
    public class StatusTicketbookService : Service<StatusTicketbook>, IStatusTicketbookService
    {

        public StatusTicketbookService(IStatusTicketbookRepository repository,
            IRequestValidationContext requestValidationContext
            ) : base(repository, requestValidationContext) { }

        public async Task InsertStatusTicketbook(CreateStatusTicketbookDTO dto)
        {
            await this.Insert(dto);
        }

        public async Task<StatusTicketbook> SelectStatusTicketbookById(Guid id)
        {
            return await _repository.SelectById(id);
        }

        public async Task<IEnumerable<StatusTicketbook>> SelectAllStatusTicketbooks()
        {
            return await _repository.SelectAll();
        }

        public async Task UpdateStatusTicketbookById(UpdateStatusTicketbookDTO dto)
        {
            await this.Update(dto);
        }

        public async Task DeleteStatusTicketbookById(Guid id)
        {
            await this.DeleteById(id);
        }
    }
}
