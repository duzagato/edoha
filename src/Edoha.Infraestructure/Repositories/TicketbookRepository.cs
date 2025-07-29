using Edoha.Domain.Entities;
using Edoha.Infrastructure.Repositories;
using System.Data;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;

namespace Edoha.Infraestructure.Repositories
{
    public class TicketbookRepository : BaseRepository<Ticketbook>, ITicketbookRepository
    {
        public TicketbookRepository(IDbConnection connection) : base(connection)
        {

        }

    }
}
