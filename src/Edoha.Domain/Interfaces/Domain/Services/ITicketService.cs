using Edoha.Domain.Entities;
using Edoha.Domain.Models.DTOs.Ticket;
using Edoha.Domain.Models.Requests.Ticket;

namespace Edoha.Domain.Interfaces.Domain.Services
{
    public interface ITicketService : IService<Ticket>
    {
        Task InsertTicket(CreateTicketRequest ticketRequest);

        public Task<Ticket> SelectTicketById(Guid id);

        public Task<IEnumerable<Ticket>> SelectAllTickets();

        public Task UpdateTicketById(UpdateTicketDTO dto);

        public Task DeleteTicketById(Guid id);
    }
}
