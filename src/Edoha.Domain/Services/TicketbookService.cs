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
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserService _userService;

        public TicketbookService(ITicketbookRepository repository, IStatusTicketbookRepository statusTicketbookRepository, ILotteryRepository lotteryRepository, 
          ITicketbookRepository ticketbookRepository,
          ITicketRepository ticketRepository,
          ILogger<ITicketbookService> logger,
          IUserService userService,
          IRequestValidationContext requestValidationContex) : base(repository, requestValidationContex) 
        {
            _logger = logger;
            _statusTicketbookRepository = statusTicketbookRepository;
            _lotteryRepository = lotteryRepository;
            _ticketbookRepository = ticketbookRepository;
            _ticketRepository = ticketRepository;
            _userService = userService;
        }

        public async Task<Guid> InsertTicketbook(PostTicketbookRequest ticketbookRequest, Guid idLottery)
        {
            _logger.LogInformation("Iniciando método InsertTicketbook");
            _logger.LogInformation("Parâmetros recebidos - idLottery: {IdLottery}, ticketbookRequest: {@TicketbookRequest}", idLottery, ticketbookRequest);
            
            await _lotteryRepository.IdExists(idLottery);
            var idStatusTicketbook = _statusTicketbookRepository.IdExists(ticketbookRequest.IdStatusTicketbook);
            
            _logger.LogInformation("Validando se o número do talão {Number} já existe na rifa {IdLottery}", ticketbookRequest.Number, idLottery);
            var ticketbookExists = await _ticketbookRepository.ValidateNumber(idLottery, ticketbookRequest.Number);

            if (ticketbookExists)
            {
                _logger.LogWarning("Talão número {Number} já existe na rifa {IdLottery}", ticketbookRequest.Number, idLottery);
                throw new Exception("O talão já existe");
            }

            _logger.LogInformation("Inserindo informações do proprietário do talão");
            var idOwner = await _userService.InsertUserInformation(ticketbookRequest.TicketbookOwner.Name, ticketbookRequest.TicketbookOwner.Phone);
            _logger.LogInformation("Proprietário inserido com Id: {IdOwner}", idOwner);

            Guid? idHolder = null;

            if(ticketbookRequest.TicketbookHolder is not null)
            {
                if (!String.IsNullOrEmpty(ticketbookRequest.TicketbookHolder.Name) &&
               !String.IsNullOrEmpty(ticketbookRequest.TicketbookHolder.Phone))
                {
                    _logger.LogInformation("Inserindo informações do portador do talão");
                    idHolder = await _userService.InsertUserInformation(ticketbookRequest.TicketbookHolder?.Name, ticketbookRequest.TicketbookHolder?.Phone);
                    _logger.LogInformation("Portador inserido com Id: {IdHolder}", idHolder);
                } 
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
                _logger.LogInformation("Status é devolvido, data de devolução definida: {DevolutionDate}", dto.DevolutionDate);
            }
            else
            {
                dto.DevolutionDate = null;
            }

            await _requestValidationContext.ValidateDTO(dto);
            
            _logger.LogInformation("Inserindo talão no repositório");
            var newId = await _repository.InsertAndReturnId(dto);
            
            _logger.LogInformation("Método InsertTicketbook finalizado. Retornando Id: {NewId}", newId);
            return newId;
        }

        public async Task<IEnumerable<Ticketbook>> SelectReturnedsTicketbooks(Guid idLottery)
        {
            _logger.LogInformation("Iniciando método SelectReturnedsTicketbooks");
            _logger.LogInformation("Parâmetros recebidos - idLottery: {IdLottery}", idLottery);
            
            await _lotteryRepository.IdExists(idLottery);
            var ticketbooks = await _ticketbookRepository.SelectReturnedsTicketbooksByLottery(idLottery);

            _logger.LogInformation("Carregando tickets para cada talão devolvido");
            foreach (var ticketbook in ticketbooks)
            {
                ticketbook.Tickets = (await _ticketRepository.SelectAllByTicketbook(ticketbook.Id)).ToList();
            }

            _logger.LogInformation("Método SelectReturnedsTicketbooks finalizado. Retornando {Count} talões devolvidos", ticketbooks.Count());
            return ticketbooks;
        }

        public async Task<IEnumerable<Ticketbook>> SelectWithdrawnsTicketbooks(Guid idLottery)
        {
            _logger.LogInformation("Iniciando método SelectWithdrawnsTicketbooks");
            _logger.LogInformation("Parâmetros recebidos - idLottery: {IdLottery}", idLottery);
            
            await _lotteryRepository.IdExists(idLottery);
            var ticketbooks = await _ticketbookRepository.SelectWithdrawnTicketbooksByLottery(idLottery);

            _logger.LogInformation("Carregando tickets para cada talão retirado");
            foreach (var ticketbook in ticketbooks)
            {
                ticketbook.Tickets = (await _ticketRepository.SelectAllByTicketbook(ticketbook.Id)).ToList();
            }

            _logger.LogInformation("Método SelectWithdrawnsTicketbooks finalizado. Retornando {Count} talões retirados", ticketbooks.Count());
            return ticketbooks;
        }

        public async Task<Ticketbook?> GetTicketbookByNumber(Guid idLottery, int numberTicketbook)
        {
            _logger.LogInformation("Iniciando método GetTicketbookByNumber");
            _logger.LogInformation("Parâmetros recebidos - idLottery: {IdLottery}, numberTicketbook: {NumberTicketbook}", idLottery, numberTicketbook);
            
            await _lotteryRepository.IdExists(idLottery);
            var ticketbook = await _ticketbookRepository.SelectTicketbookByNumber(idLottery, numberTicketbook);

            if (ticketbook is not null)
            {
                _logger.LogInformation("Talão {TicketbookId} encontrado, carregando tickets associados", ticketbook.Id);
                ticketbook.Tickets = (await _ticketRepository.SelectAllByTicketbook(ticketbook.Id)).ToList();
                _logger.LogInformation("Método GetTicketbookByNumber finalizado. Retornando talão {TicketbookId} com {TicketCount} tickets", ticketbook.Id, ticketbook.Tickets.Count);
            }
            else
            {
                _logger.LogInformation("Método GetTicketbookByNumber finalizado. Nenhum talão encontrado");
            }

            return ticketbook;
        }

        public async Task ChangeTicketbookStatus(int idStatusTicketbook, Guid idTicketbook, Guid idLottery)
        {
            _logger.LogInformation("Iniciando método ChangeTicketbookStatus");
            _logger.LogInformation("Parâmetros recebidos - idLottery: {IdLottery}, idTicketbook: {IdTicketbook}, idStatusTicketbook: {IdStatusTicketbook}", idLottery, idTicketbook, idStatusTicketbook);
            
            await _lotteryRepository.IdExists(idLottery);
            _logger.LogInformation("Alterando o status do Talão");
            _logger.LogInformation("ID novo status: {Id}", idStatusTicketbook);

            await _ticketbookRepository.UpdateStatus(idStatusTicketbook, idTicketbook);
            
            _logger.LogInformation("Método ChangeTicketbookStatus finalizado com sucesso");
        }

        public async Task ChangeTicketbookStatusToWithdraw(Guid idTicketbook, Guid idLottery)
        {
            _logger.LogInformation("Iniciando método ChangeTicketbookStatusToWithdraw");
            _logger.LogInformation("Parâmetros recebidos - idLottery: {IdLottery}, idTicketbook: {IdTicketbook}", idLottery, idTicketbook);
            
            await _lotteryRepository.IdExists(idLottery);
            _logger.LogInformation("Alterando o status do Talão para retirado");
            _logger.LogInformation("ID do talão: {Ticketbook}", idTicketbook);

            await _ticketbookRepository.UpdateStatusToWithdraw(idTicketbook);
            
            _logger.LogInformation("Método ChangeTicketbookStatusToWithdraw finalizado com sucesso");
        }

        public async Task ChangeTicketbookStatusToReturned(Guid idTicketbook, Guid idLottery)
        {
            _logger.LogInformation("Iniciando método ChangeTicketbookStatusToReturned");
            _logger.LogInformation("Parâmetros recebidos - idLottery: {IdLottery}, idTicketbook: {IdTicketbook}", idLottery, idTicketbook);
            
            await _lotteryRepository.IdExists(idLottery);
            _logger.LogInformation("Alterando o status do Talão para devolvido");
            _logger.LogInformation("ID do talão: {Ticketbook}", idTicketbook);

            await _ticketbookRepository.UpdateStatusToReturned(idTicketbook);
            
            _logger.LogInformation("Método ChangeTicketbookStatusToReturned finalizado com sucesso");
        }

        public async Task<Ticketbook> SelectTicketbookById(Guid id, Guid idLottery)
        {
            _logger.LogInformation("Iniciando método SelectTicketbookById");
            _logger.LogInformation("Parâmetros recebidos - idLottery: {IdLottery}, id: {Id}", idLottery, id);
            
            await _lotteryRepository.IdExists(idLottery);
            var ticketbook = await _repository.SelectById(id);
            
            _logger.LogInformation("Talão {TicketbookId} encontrado, carregando tickets associados", ticketbook.Id);
            ticketbook.Tickets = (await _ticketRepository.SelectAllByTicketbook(ticketbook.Id)).ToList();
            
            _logger.LogInformation("Método SelectTicketbookById finalizado. Retornando talão {TicketbookId} com {TicketCount} tickets", ticketbook.Id, ticketbook.Tickets.Count);
            return ticketbook;
        }

        public async Task<IEnumerable<Ticketbook>> SelectAllTicketbooks(Guid idLottery)
        {
            _logger.LogInformation("Iniciando método SelectAllTicketbooks");
            _logger.LogInformation("Parâmetros recebidos - idLottery: {IdLottery}", idLottery);
            
            await _lotteryRepository.IdExists(idLottery);
            var ticketbooks = await _ticketbookRepository.SelectAll(idLottery);

            _logger.LogInformation("Método SelectAllTicketbooks finalizado. Retornando {Count} talões", ticketbooks.Count());
            return ticketbooks;
        }

        public async Task UpdateTicketbookById(UpdateTicketbookDTO dto, Guid idLottery)
        {
            _logger.LogInformation("Iniciando método UpdateTicketbookById");
            _logger.LogInformation("Parâmetros recebidos - idLottery: {IdLottery}, dto: {@Dto}", idLottery, dto);
            
            await _lotteryRepository.IdExists(idLottery);
            await _statusTicketbookRepository.IdExists(dto.IdStatusTicketbook);
            
            await this.Update(dto);
            
            _logger.LogInformation("Método UpdateTicketbookById finalizado com sucesso");
        }

        public async Task DeleteTicketbookById(Guid id, Guid idLottery)
        {
            _logger.LogInformation("Iniciando método DeleteTicketbookById");
            _logger.LogInformation("Parâmetros recebidos - idLottery: {IdLottery}, id: {Id}", idLottery, id);
            
            await _lotteryRepository.IdExists(idLottery);
            await this.DeleteById(id);
            
            _logger.LogInformation("Método DeleteTicketbookById finalizado com sucesso");
        }
    }
}
