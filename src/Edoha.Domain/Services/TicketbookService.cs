using Edoha.Domain.Entities;
using Edoha.Domain.Models.DTOs.Ticketbook;
using Edoha.Domain.Interfaces.Infraestructure.Context;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Interfaces.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Edoha.Domain.Services
{
    public class TicketbookService : Service<Ticketbook>, ITicketbookService
    {
        private readonly ILogger<ITicketbookService> _logger;
        private readonly IStatusTicketbookRepository _statusTicketbookRepository;
        private readonly ILotteryRepository _lotteryRepository;
        private readonly ITicketbookRepository _ticketbookRepository;

        public TicketbookService(ITicketbookRepository repository, IStatusTicketbookRepository statusTicketbookRepository, ILotteryRepository lotteryRepository, 
          ITicketbookRepository ticketbookRepository,
          ILogger<ITicketbookService> logger,
          IRequestValidationContext requestValidationContex) : base(repository, requestValidationContex) 
        {
            _logger = logger;
            _statusTicketbookRepository = statusTicketbookRepository;
            _lotteryRepository = lotteryRepository;
            _ticketbookRepository = ticketbookRepository;
        }

        public async Task InsertTicketbook(CreateTicketbookDTO dto)
        {
            await _lotteryRepository.IdExists(dto.IdLottery);
            await _statusTicketbookRepository.IdExists(dto.IdStatusTicketbook);
            await this.Insert(dto);
        }

        public async Task<IEnumerable<Ticketbook>> SelectReturnedsTicketbooks(Guid idLottery)
        {
            var ticketbooks = await _ticketbookRepository.SelectReturnedsTicketbooksByLottery(idLottery);

            return ticketbooks;
        }

        public async Task<IEnumerable<Ticketbook>> SelectWithdrawnsTicketbooks(Guid idLottery)
        {
            var ticketbooks = await _ticketbookRepository.SelectWithdrawnTicketbooksByLottery(idLottery);

            return ticketbooks;
        }

        public async Task ChangeTicketbookStatus(int idStatusTicketbook, Guid idTicketbook)
        {
            _logger.LogInformation("Alterando o status do Talão");
            _logger.LogInformation("ID novo status: {Id}", idStatusTicketbook);

            await _ticketbookRepository.UpdateStatus(idStatusTicketbook, idTicketbook);
        }

        public async Task ChangeTicketbookStatusToWithdraw(Guid idTicketbook)
        {
            _logger.LogInformation("Alterando o status do Talão para retirado");
            _logger.LogInformation("ID do talão: {Ticketbook}", idTicketbook);

            await _ticketbookRepository.UpdateStatusToWithdraw(idTicketbook);
        }

        public async Task ChangeTicketbookStatusToReturned(Guid idTicketbook)
        {
            _logger.LogInformation("Alterando o status do Talão para devolvido");
            _logger.LogInformation("ID do talão: {Ticketbook}", idTicketbook);

            await _ticketbookRepository.UpdateStatusToReturned(idTicketbook);
        }

        public async Task<Ticketbook> SelectTicketbookById(Guid id)
        {
            return await _repository.SelectById(id);
        }

        public async Task<IEnumerable<Ticketbook>> SelectAllTicketbooks()
        {
            return await _repository.SelectAll();
        }

        public async Task UpdateTicketbookById(UpdateTicketbookDTO dto)
        {
            await _statusTicketbookRepository.IdExists(dto.IdStatusTicketbook);
            await this.Update(dto);
        }

        public async Task DeleteTicketbookById(Guid id)
        {
            await this.DeleteById(id);
        }
    }
}
