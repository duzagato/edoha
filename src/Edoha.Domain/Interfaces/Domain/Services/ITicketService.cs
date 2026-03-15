using Edoha.Domain.Entities;
using Edoha.Domain.Models.DTOs.Ticket;
using Edoha.Domain.Models.Requests.Ticket;

namespace Edoha.Domain.Interfaces.Domain.Services
{
    public interface ITicketService : IService<Ticket>
    {
        Task InsertTicket(Guid idTicketbook, CreateTicketRequest ticketRequest);

        public Task<Ticket> SelectTicketById(Guid idTicketbook, Guid id);

        public Task<IEnumerable<Ticket>> SelectAllTickets(Guid idTicketbook);

        public Task UpdateTicketById(Guid idTicketbook, UpdateTicketDTO dto);

        public Task DeleteTicketById(Guid idTicketbook, Guid id);
    }
}
