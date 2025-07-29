using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Models.DTOs.Ticketbook;

namespace Edoha.Domain.Interfaces.Domain.Services
{
    public interface ITicketbookService : IService<Ticketbook>
    {
        public Task InsertTicketbook(CreateTicketbookDTO dto);

        public Task<Ticketbook> SelectTicketbookById(Guid id);

        public Task<IEnumerable<Ticketbook>> SelectAllTicketbooks();

        public Task UpdateTicketbookById(UpdateTicketbookDTO dto);

        public Task DeleteTicketbookById(Guid id);
    }
}
