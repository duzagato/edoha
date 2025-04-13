using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces;
using Edoha.Infraestructure.Helpers;
using Edoha.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Infraestructure.Repositories
{
    public class StatusTicketbookRepository : BaseRepository<StatusTicketbook>, IStatusTicketbookRepository
    {
        public StatusTicketbookRepository(IDbConnection connection) : base(connection) 
        { 
            
        }

    }
}
