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
using Dapper;

namespace Edoha.Infraestructure.Repositories
{
    public class TicketbookRepository : BaseRepository<Ticketbook>, ITicketbookRepository
    {
        public TicketbookRepository(IDbConnection connection) : base(connection)
        {

        }

        public async Task<Ticketbook?> SelectTicketbookByNumber(Guid idLottery, int numberTicketbook)
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            CheckConnection();

            return await _connection.QueryFirstOrDefaultAsync<Ticketbook>(
                StaticQueries.SelectTicketbookByNumber,
                new { IdLottery = idLottery, Number = numberTicketbook });
        }
    }
}
