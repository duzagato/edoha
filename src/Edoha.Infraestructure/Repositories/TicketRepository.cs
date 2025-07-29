using Edoha.Domain.Entities;
using Edoha.Infrastructure.Repositories;
using System.Data;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;

namespace Edoha.Infraestructure.Repositories
{
    public class TicketRepository : BaseRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(IDbConnection connection) : base(connection) 
        { 
            
        }

    }
}
