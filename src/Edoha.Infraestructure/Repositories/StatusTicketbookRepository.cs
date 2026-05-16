using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Infraestructure.Factories;
using Edoha.Infrastructure.Repositories;
using System.Data;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;

namespace Edoha.Infraestructure.Repositories
{
    public class StatusTicketbookRepository : BaseRepository<StatusTicketbook>, IStatusTicketbookRepository
    {
        public StatusTicketbookRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory) 
        { 
            
        }

    }
}
