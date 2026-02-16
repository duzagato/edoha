using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Models.DTOs.Ticketbook;
using Edoha.Domain.Models.Requests.Ticketbook;

namespace Edoha.Domain.Interfaces.Domain.Services
{
    public interface ITicketbookService : IService<Ticketbook>
    {
        public Task InsertTicketbook(CreateTicketbookDTO dto);

        public Task<IEnumerable<Ticketbook>> SelectReturnedsTicketbooks(Guid idLottery);

        public Task<IEnumerable<Ticketbook>> SelectWithdrawnsTicketbooks(Guid idLottery);

        public Task ChangeTicketbookStatus(int idStatusTicketbook, Guid idTicketbook);

        public Task<Ticketbook> SelectTicketbookById(Guid id);

        public Task<IEnumerable<Ticketbook>> SelectAllTicketbooks();

        public Task UpdateTicketbookById(UpdateTicketbookDTO dto);

        public Task DeleteTicketbookById(Guid id);
    }
}
