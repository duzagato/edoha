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
        private readonly IUserRepository _userRepository;

        public TicketbookService(ITicketbookRepository repository, IStatusTicketbookRepository statusTicketbookRepository, ILotteryRepository lotteryRepository, 
          ITicketbookRepository ticketbookRepository,
          IUserRepository userRepository,
          ILogger<ITicketbookService> logger,
          IRequestValidationContext requestValidationContex) : base(repository, requestValidationContex) 
        {
            _logger = logger;
            _statusTicketbookRepository = statusTicketbookRepository;
            _lotteryRepository = lotteryRepository;
            _ticketbookRepository = ticketbookRepository;
            _userRepository = userRepository;
        }

        private async Task<TicketbookResponse> BuildTicketbookResponse(Ticketbook ticketbook)
        {
            var lottery = await _lotteryRepository.SelectById(ticketbook.IdLottery);
            var statusTicketbook = await _statusTicketbookRepository.SelectByCode(ticketbook.IdStatusTicketbook);

            User? owner = ticketbook.IdOwner.HasValue
                ? await _userRepository.SelectById(ticketbook.IdOwner.Value)
                : null;

            User? holder = ticketbook.IdHolder.HasValue
                ? await _userRepository.SelectById(ticketbook.IdHolder.Value)
                : null;

            User? createdBy = ticketbook.CreatedBy.HasValue
                ? await _userRepository.SelectById(ticketbook.CreatedBy.Value)
                : null;

            return new TicketbookResponse
            {
                Id = ticketbook.Id,
                Lottery = lottery,
                Owner = owner,
                Holder = holder,
                StatusTicketbook = statusTicketbook,
                Number = ticketbook.Number,
                WithdrawnDate = ticketbook.WithdrawnDate,
                DevolutionDate = ticketbook.DevolutionDate,
                CreatedAt = ticketbook.CreatedAt,
                CreatedBy = createdBy
            };
        }

        public async Task InsertTicketbook(CreateTicketbookDTO dto)
        {
            await _lotteryRepository.IdExists(dto.IdLottery);
            await _statusTicketbookRepository.IdExists(dto.IdStatusTicketbook);
            await this.Insert(dto);
        }

        public async Task<IEnumerable<TicketbookResponse>> SelectReturnedsTicketbooks(Guid idLottery)
        {
            var ticketbooks = await _ticketbookRepository.SelectReturnedsTicketbooksByLottery(idLottery);
            var responses = new List<TicketbookResponse>();
            foreach (var ticketbook in ticketbooks)
                responses.Add(await BuildTicketbookResponse(ticketbook));
            return responses;
        }

        public async Task<IEnumerable<TicketbookResponse>> SelectWithdrawnsTicketbooks(Guid idLottery)
        {
            var ticketbooks = await _ticketbookRepository.SelectWithdrawnTicketbooksByLottery(idLottery);
            var responses = new List<TicketbookResponse>();
            foreach (var ticketbook in ticketbooks)
                responses.Add(await BuildTicketbookResponse(ticketbook));
            return responses;
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

        public async Task<TicketbookResponse> SelectTicketbookById(Guid id)
        {
            var ticketbook = await _repository.SelectById(id);
            return await BuildTicketbookResponse(ticketbook);
        }

        public async Task<IEnumerable<TicketbookResponse>> SelectAllTicketbooks()
        {
            var ticketbooks = await _repository.SelectAll();
            var responses = new List<TicketbookResponse>();
            foreach (var ticketbook in ticketbooks)
                responses.Add(await BuildTicketbookResponse(ticketbook));
            return responses;
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
