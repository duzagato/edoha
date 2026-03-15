using Edoha.Domain.Entities;
using Edoha.Domain.Models.DTOs.Ticketbook;

namespace Edoha.Domain.Interfaces.Infraestructure.Repositories
{
    public interface ITicketbookRepository : IBaseRepository<Ticketbook>
    {
        public Task<IEnumerable<Ticketbook>> SelectReturnedsTicketbooksByLottery(Guid idLottery);

        public Task<IEnumerable<Ticketbook>> SelectWithdrawnTicketbooksByLottery(Guid IdLottery);

        public Task UpdateStatus(int idStatusTicketbook, Guid idTicketbook);

        public Task UpdateStatusToReturned(Guid idTicketbook);

        public Task UpdateStatusToWithdraw(Guid idTicketbook);

        Task<bool> ValidateNumber(Guid idLottery, int number);

        Task<TicketbookConfigurationDTO?> SelectTicketbookConfiguration(Guid IdTicketbook);
        Task<Ticketbook?> SelectTicketbookByNumber(Guid idLottery, int numberTicketbook);
    }
}
