using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Models.DTOs.Ticketbook;
using Edoha.Domain.Models.Requests.Ticketbook;

namespace Edoha.Domain.Interfaces.Domain.Services
{
    public interface ITicketbookService : IService<Ticketbook>
    {
        Task InsertTicketbook(PostTicketbookRequest ticketbookRequest);

        Task<IEnumerable<Ticketbook>> SelectReturnedsTicketbooks(Guid idLottery);

        Task<IEnumerable<Ticketbook>> SelectWithdrawnsTicketbooks(Guid idLottery);

        Task ChangeTicketbookStatus(int idStatusTicketbook, Guid idTicketbook);

        Task ChangeTicketbookStatusToReturned(Guid idTicketbook);

        Task ChangeTicketbookStatusToWithdraw(Guid idTicketbook);

        Task<Ticketbook> SelectTicketbookById(Guid id);

        Task<IEnumerable<Ticketbook>> SelectAllTicketbooks();

        Task UpdateTicketbookById(UpdateTicketbookDTO dto);

        Task DeleteTicketbookById(Guid id);
    }
}
