using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edoha.Domain.Entities;

namespace Edoha.Domain.Interfaces.Infraestructure.Repositories
{
    public interface ITicketbookRepository : IBaseRepository<Ticketbook>
    {
        public Task<IEnumerable<Ticketbook>> SelectReturnedsTicketbooksByLottery(Guid idLottery);

        public Task<IEnumerable<Ticketbook>> SelectWithdrawnTicketbooksByLottery(Guid IdLottery);

        public Task UpdateStatus(Guid idStatusTicketbook, Guid idTicketbook);
    }
}
