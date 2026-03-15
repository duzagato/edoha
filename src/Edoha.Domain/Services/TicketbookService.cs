using Edoha.Domain.Constants.Enums;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Domain.Services;
using Edoha.Domain.Interfaces.Infraestructure.Context;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Models.DTOs.Ticketbook;
using Edoha.Domain.Models.Requests.Ticketbook;
using Microsoft.Extensions.Logging;
using System.Numerics;
using System.Xml.Linq;

namespace Edoha.Domain.Services
{
    public class TicketbookService : Service<Ticketbook>, ITicketbookService
    {
        private readonly ILogger<ITicketbookService> _logger;
        private readonly IStatusTicketbookRepository _statusTicketbookRepository;
        private readonly ILotteryRepository _lotteryRepository;
        private readonly ITicketbookRepository _ticketbookRepository;
        private readonly IUserService _userService;

        public TicketbookService(ITicketbookRepository repository, IStatusTicketbookRepository statusTicketbookRepository, ILotteryRepository lotteryRepository, 
          ITicketbookRepository ticketbookRepository,
          ILogger<ITicketbookService> logger,
          IUserService userService,
          IRequestValidationContext requestValidationContex) : base(repository, requestValidationContex) 
        {
            _logger = logger;
            _statusTicketbookRepository = statusTicketbookRepository;
            _lotteryRepository = lotteryRepository;
            _ticketbookRepository = ticketbookRepository;
            _userService = userService;
        }

        public async Task InsertTicketbook(PostTicketbookRequest ticketbookRequest, Guid idLottery)
        {
            await _lotteryRepository.IdExists(idLottery);
            var idStatusTicketbook = _statusTicketbookRepository.IdExists(ticketbookRequest.IdStatusTicketbook);
            var ticketbookExists = await _ticketbookRepository.ValidateNumber(idLottery, ticketbookRequest.Number);

            if (ticketbookExists)
            {
                throw new Exception("O talão já existe");
            }


            var idOwner = await _userService.InsertUserInformation(ticketbookRequest.TicketbookOwner.Name, ticketbookRequest.TicketbookOwner.Phone);

            Guid? idHolder = null;

            if (String.IsNullOrEmpty(ticketbookRequest.TicketbookHolder.Name) &&
               String.IsNullOrEmpty(ticketbookRequest.TicketbookHolder.Phone))
            {
                idHolder = await _userService.InsertUserInformation(ticketbookRequest.TicketbookHolder?.Name, ticketbookRequest.TicketbookHolder?.Phone);
            }

                CreateTicketbookDTO dto = new CreateTicketbookDTO
                {
                    IdLottery = idLottery,
                    Number = ticketbookRequest.Number,
                    IdOwner = idOwner,
                    IdHolder = idHolder ?? null,
                    WithdrawnDate = DateTime.Now,
                    IdStatusTicketbook = ticketbookRequest.IdStatusTicketbook
                };

            if (ticketbookRequest.IdStatusTicketbook is (int)StatusTicketbookEnum.Devolvido)
            {
                dto.DevolutionDate = DateTime.Now;
            }
            else
            {
                dto.DevolutionDate = null;
            }

            await Insert(dto);
        }

        public async Task<IEnumerable<Ticketbook>> SelectReturnedsTicketbooks(Guid idLottery)
        {
            await _lotteryRepository.IdExists(idLottery);
            var ticketbooks = await _ticketbookRepository.SelectReturnedsTicketbooksByLottery(idLottery);

            return ticketbooks;
        }

        public async Task<IEnumerable<Ticketbook>> SelectWithdrawnsTicketbooks(Guid idLottery)
        {
            await _lotteryRepository.IdExists(idLottery);
            var ticketbooks = await _ticketbookRepository.SelectWithdrawnTicketbooksByLottery(idLottery);

            return ticketbooks;
        }

        public async Task ChangeTicketbookStatus(int idStatusTicketbook, Guid idTicketbook, Guid idLottery)
        {
            await _lotteryRepository.IdExists(idLottery);
            _logger.LogInformation("Alterando o status do Talão");
            _logger.LogInformation("ID novo status: {Id}", idStatusTicketbook);

            await _ticketbookRepository.UpdateStatus(idStatusTicketbook, idTicketbook);
        }

        public async Task ChangeTicketbookStatusToWithdraw(Guid idTicketbook, Guid idLottery)
        {
            await _lotteryRepository.IdExists(idLottery);
            _logger.LogInformation("Alterando o status do Talão para retirado");
            _logger.LogInformation("ID do talão: {Ticketbook}", idTicketbook);

            await _ticketbookRepository.UpdateStatusToWithdraw(idTicketbook);
        }

        public async Task ChangeTicketbookStatusToReturned(Guid idTicketbook, Guid idLottery)
        {
            await _lotteryRepository.IdExists(idLottery);
            _logger.LogInformation("Alterando o status do Talão para devolvido");
            _logger.LogInformation("ID do talão: {Ticketbook}", idTicketbook);

            await _ticketbookRepository.UpdateStatusToReturned(idTicketbook);
        }

        public async Task<Ticketbook> SelectTicketbookById(Guid id, Guid idLottery)
        {
            await _lotteryRepository.IdExists(idLottery);
            return await _repository.SelectById(id);
        }

        public async Task<IEnumerable<Ticketbook>> SelectAllTicketbooks(Guid idLottery)
        {
            await _lotteryRepository.IdExists(idLottery);
            return await _repository.SelectAll();
        }

        public async Task UpdateTicketbookById(UpdateTicketbookDTO dto, Guid idLottery)
        {
            await _lotteryRepository.IdExists(idLottery);
            await _statusTicketbookRepository.IdExists(dto.IdStatusTicketbook);
            await this.Update(dto);
        }

        public async Task DeleteTicketbookById(Guid id, Guid idLottery)
        {
            await _lotteryRepository.IdExists(idLottery);
            await this.DeleteById(id);
        }
    }
}
