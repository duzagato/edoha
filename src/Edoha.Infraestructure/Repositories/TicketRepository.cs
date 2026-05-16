using Dapper;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Infraestructure.Factories;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Infraestructure.Constants;
using Edoha.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Edoha.Infraestructure.Repositories
{
    public class TicketRepository : BaseRepository<Ticket>, ITicketRepository
    {
        private readonly ILogger<ITicketRepository> _logger;
        public TicketRepository(ILogger<ITicketRepository> logger, 
            IDbConnectionFactory connectionFactory) : base(connectionFactory) 
        {
            _logger = logger;
        }

        public async Task<bool> TicketExists(Guid idTicketbook, int number)
        {
            try
            {
                CheckConnection();

                _logger.LogInformation("Executando query para validar se a combinação talão e número de talão já existe");

                string query = StaticQueries.TicketExists;

                _logger.LogInformation("IdTicketbook: {IdTicketbook}", idTicketbook);
                _logger.LogInformation("Número Bilhete: {Number}", number);

                bool exists = await _connection.QueryFirstAsync<bool>(query, new
                {
                    IdTicketbook = idTicketbook,
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

        public async Task<IEnumerable<Ticket>> SelectAllByTicketbook(Guid idTicketbook)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            CheckConnection();

            _logger.LogInformation("Executando query para listar bilhetes do talão {IdTicketbook}", idTicketbook);

            string query = StaticQueries.SelectAllTicketsByTicketbook;

            var tickets = await _connection.QueryAsync<Ticket>(query, new { IdTicketbook = idTicketbook });

            _logger.LogInformation("Query executada.");

            return tickets;
        }

        public async Task<Ticket> SelectByIdAndTicketbook(Guid id, Guid idTicketbook)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            CheckConnection();

            _logger.LogInformation("Executando query para buscar bilhete {Id} do talão {IdTicketbook}", id, idTicketbook);

            string query = StaticQueries.SelectTicketByIdAndTicketbook;

            var ticket = await _connection.QueryFirstOrDefaultAsync<Ticket>(query, new
            {
                Id = id,
                IdTicketbook = idTicketbook
            });

            return ticket ?? throw new KeyNotFoundException("Entidade não encontrada");
        }
    }
}
