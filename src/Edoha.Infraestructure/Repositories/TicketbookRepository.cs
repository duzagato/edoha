using Dapper;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Infraestructure.Factories;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Infraestructure.Constants;
using Edoha.Domain.Constants.Enums;
using Edoha.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using System.Data;
using Edoha.Domain.Models.DTOs.Ticketbook;

namespace Edoha.Infraestructure.Repositories
{
    public class TicketbookRepository : BaseRepository<Ticketbook>, ITicketbookRepository
    {
        public ILogger<TicketbookRepository> _logger;
        public TicketbookRepository(IDbConnectionFactory connectionFactory, ILogger<TicketbookRepository> logger) : base(connectionFactory)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            _logger = logger;
        }

        public async Task<IEnumerable<Ticketbook>> SelectReturnedsTicketbooksByLottery(Guid idLottery)
        {
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

        public async Task<bool> ValidateNumber(Guid idLottery, int number)
        {
            try
            {
                CheckConnection();

                _logger.LogInformation("Executando query para validar se a combinação rifa e número de talão já existe");

                string query = StaticQueries.LotteryTicketbookNumberExists;

                _logger.LogInformation("IdLottery: {IdLottery}", idLottery);
                _logger.LogInformation("Número Talão: {Number}", number);

                bool exists = await _connection.QueryFirstAsync<bool>(query, new
                {
                    IdLottery = idLottery,
                    Number = number
                });

                _logger.LogInformation("Query executada.");

                return exists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao realizar alteração de status no banco de dados");
                throw;
            }
        }

        public async Task<TicketbookConfigurationDTO?> SelectTicketbookConfiguration(Guid idTicketbook)
        {
            try
            {
                _logger.LogInformation("Recebendo configurações de talão e rifa");
                _logger.LogInformation("ID Talão que será consultado: {IdTicketbook}", idTicketbook);

                string query = StaticQueries.TicketbookConfiguration;

                var ticketbookConfiguration = await _connection.QueryFirstOrDefaultAsync<TicketbookConfigurationDTO?>(query, new { IdTicketbook = idTicketbook });

                _logger.LogInformation("Configurações do talão recebida com sucesso!");

                return ticketbookConfiguration;
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao tentar receber configurações do talão");
                throw;
            }
        }

        public async Task<IEnumerable<Ticketbook>> SelectAll(Guid idLottery)
        {
            CheckConnection();

            var result = await _connection.QueryAsync<Ticketbook, Holder, Owner, Ticketbook>(
                StaticQueries.SelectAllTicketbooks,
                (ticketbook, holder, owner) =>
                {
                    ticketbook.TicketbookHolder = (holder?.Id != null) ? holder : null;
                    ticketbook.TicketbookOwner = owner;
                    return ticketbook;
                },
                new { IdLottery = idLottery },
                splitOn: "id,id"
            );

            return result;
        }

        public async Task<Ticketbook?> SelectTicketbookByNumber(Guid idLottery, int numberTicketbook)
        {
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            CheckConnection();

            // O Dapper espera: <Tipo1, Tipo2, Tipo3, TipoRetorno>
            var result = await _connection.QueryAsync<Ticketbook, Holder, Owner, Ticketbook>(
                StaticQueries.SelectTicketbookByNumber,
                (ticketbook, holder, owner) =>
                {
                    ticketbook.TicketbookHolder = holder;
                    ticketbook.TicketbookOwner = owner;
                    return ticketbook;
                },
                new { IdLottery = idLottery, Number = numberTicketbook },
                splitOn: "Id,Id" // Indica que quando encontrar a coluna "Id", começa um novo objeto
            );

            return result.FirstOrDefault();
        }
    }
}
