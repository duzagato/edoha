using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Models.DTOs.Ticket;

namespace Edoha.Domain.Interfaces.Domain.Services
{
    public interface ITicketService : IService<Ticket>
    {
        public Task InsertTicket(CreateTicketDTO dto);

        public Task<Ticket> SelectTicketById(Guid id);

        public Task<IEnumerable<Ticket>> SelectAllTickets();

        public Task UpdateTicketById(UpdateTicketDTO dto);

        public Task DeleteTicketById(Guid id);
    }
}
