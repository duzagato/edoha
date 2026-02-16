using Dapper;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Models.DTOs.UserPermission;
using Edoha.Infraestructure.Constants;
using Edoha.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Edoha.Infraestructure.Repositories
{
    public class TicketbookRepository : BaseRepository<Ticketbook>, ITicketbookRepository
    {
        public static string ReturnedStatus = "Devolvido";
        public static string WithdrawnStatus = "Retirado";
        public ILogger<TicketbookRepository> _logger;
        public TicketbookRepository(IDbConnection connection, ILogger<TicketbookRepository> logger) : base(connection)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<Ticketbook>> SelectReturnedsTicketbooksByLottery(Guid idLottery)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            CheckConnection();

            _logger.LogInformation("Recebendo talões devolvidos");

            string query = StaticQueries.SelectReturnedsTicketbooksByLottery;

            _logger.LogInformation("Query: {query}", query);

            var ticketbooks = await _connection.QueryAsync<Ticketbook>(query, new 
                {
                    IdLottery = idLottery, 
                    NameStatusTicketbook = ReturnedStatus
                });

            _logger.LogInformation("Retorno consulta: {Ticketbooks}", ticketbooks);

            return ticketbooks;
        }

        public async Task<IEnumerable<Ticketbook>> SelectWithdrawnTicketbooksByLottery(Guid idLottery)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            CheckConnection();

            _logger.LogInformation("Recebendo talões retirados");

            string query = StaticQueries.SelectWithdrawnTicketbooksByLottery;

            _logger.LogInformation("Query: {query}", query);

            var ticketbooks = await _connection.QueryAsync<Ticketbook>(query, new
            {
                IdLottery = idLottery,
                NameStatusTicketbook = WithdrawnStatus
            });

            _logger.LogInformation("Retorno consulta: {Ticketbooks}", ticketbooks);

            return ticketbooks;
        }

        public async Task UpdateStatus(Guid idStatusTicketbook, Guid idTicketbook)
        {
            try
            {
                CheckConnection();

                _logger.LogInformation("Executando query para alterar status do talão");

                string query = StaticQueries.UpdateStatusTicketbook;

                _logger.LogInformation("Query: {query}", query);
                _logger.LogInformation("ID talão: {IdTicketbook}", idTicketbook);
                _logger.LogInformation("ID status: {IdStatus}", idStatusTicketbook);

                var queryResult = await _connection.ExecuteAsync(query, new
                {
                    IdStatusTicketbook = idStatusTicketbook,
                    IdTicketbook = idTicketbook
                });

                _logger.LogInformation("Query executada. Linhas afetadas: {queryResult}", queryResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao realizar alteração de status no banco de dados");
                throw;
            }
        }
    }
}
