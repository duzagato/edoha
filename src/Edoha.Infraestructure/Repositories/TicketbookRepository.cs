using Dapper;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Models.DTOs.UserPermission;
using Edoha.Infraestructure.Constants;
using Edoha.Infraestructure.Constants.Enums;
using Edoha.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Edoha.Infraestructure.Repositories
{
    public class TicketbookRepository : BaseRepository<Ticketbook>, ITicketbookRepository
    {
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

            string query = StaticQueries.SelectTicketbookByStatus;

            _logger.LogInformation("Query: {query}", query);

            var ticketbooks = await _connection.QueryAsync<Ticketbook>(query, new 
                {
                    IdLottery = idLottery, 
                    IdStatusTicketbook = StatusTicketbookEnum.Devolvido
                });

            _logger.LogInformation("Retorno consulta: {Ticketbooks}", ticketbooks);

            return ticketbooks;
        }

        public async Task<IEnumerable<Ticketbook>> SelectWithdrawnTicketbooksByLottery(Guid idLottery)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            CheckConnection();

            _logger.LogInformation("Recebendo talões retirados");

            string query = StaticQueries.SelectTicketbookByStatus;

            _logger.LogInformation("Query: {query}", query);

            var ticketbooks = await _connection.QueryAsync<Ticketbook>(query, new
            {
                IdLottery = idLottery,
                IdStatusTicketbook = StatusTicketbookEnum.Retirado
            });

            _logger.LogInformation("Retorno consulta: {Ticketbooks}", ticketbooks);

            return ticketbooks;
        }

        public async Task UpdateStatus(int idStatusTicketbook, Guid idTicketbook)
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

        public async Task UpdateStatusToReturned(Guid idTicketbook)
        {
            try
            {
                CheckConnection();

                _logger.LogInformation("Executando query para alterar status do talão");

                string query = StaticQueries.UpdateStatusTicketbookToReturned;

                _logger.LogInformation("Query: {query}", query);
                _logger.LogInformation("ID talão: {IdTicketbook}", idTicketbook);

                var queryResult = await _connection.ExecuteAsync(query, new
                {
                    IdStatusTicketbook = StatusTicketbookEnum.Devolvido,
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

        public async Task UpdateStatusToWithdraw(Guid idTicketbook)
        {
            try
            {
                CheckConnection();

                _logger.LogInformation("Executando query para alterar status do talão para retirado");

                string query = StaticQueries.UpdateStatusTicketbookToWithdraw;

                _logger.LogInformation("Query: {query}", query);
                _logger.LogInformation("ID talão: {IdTicketbook}", idTicketbook);

                var queryResult = await _connection.ExecuteAsync(query, new
                {
                    IdStatusTicketbook = StatusTicketbookEnum.Retirado,
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
