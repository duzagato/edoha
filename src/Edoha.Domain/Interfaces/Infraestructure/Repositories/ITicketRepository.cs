using Edoha.Domain.Entities;

namespace Edoha.Domain.Interfaces.Infraestructure.Repositories
{
    public interface ITicketRepository : IBaseRepository<Ticket>
    {
        Task<bool> TicketExists(Guid idTicketbook, int number);
    }
}
