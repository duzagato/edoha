using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Models.DTOs.Ticketbook;
using Edoha.Domain.Models.Requests.Ticketbook;

namespace Edoha.Domain.Interfaces.Domain.Services
{
    public interface ITicketbookService : IService<Ticketbook>
    {
        Task InsertTicketbook(PostTicketbookRequest ticketbookRequest, Guid idLottery);

        Task<IEnumerable<Ticketbook>> SelectReturnedsTicketbooks(Guid idLottery);

        Task<IEnumerable<Ticketbook>> SelectWithdrawnsTicketbooks(Guid idLottery);

        Task<Ticketbook?> GetTicketbookByNumber(Guid idLottery, int numberTicketbook);

        Task ChangeTicketbookStatus(int idStatusTicketbook, Guid idTicketbook, Guid idLottery);

        Task ChangeTicketbookStatusToReturned(Guid idTicketbook, Guid idLottery);

        Task ChangeTicketbookStatusToWithdraw(Guid idTicketbook, Guid idLottery);

        Task<Ticketbook> SelectTicketbookById(Guid id, Guid idLottery);

        Task<IEnumerable<Ticketbook>> SelectAllTicketbooks(Guid idLottery);

        Task UpdateTicketbookById(UpdateTicketbookDTO dto, Guid idLottery);

        Task DeleteTicketbookById(Guid id, Guid idLottery);
    }
}
