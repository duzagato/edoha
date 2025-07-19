using Edoha.Domain.Entities;
using Edoha.Infraestructure.Helpers;
using Edoha.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edoha.Domain.Interfaces.Repositories;

namespace Edoha.Infraestructure.Repositories
{
    public class TicketbookRepository : BaseRepository<Ticketbook>, ITicketbookRepository
    {
        public TicketbookRepository(IDbConnection connection) : base(connection)
        {

        }

    }
}
