using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Domain.Services;
using Edoha.Domain.Interfaces.Infraestructure.Context;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Models.DTOs.Ticket;
using Edoha.Domain.Models.DTOs.Ticketbook;
using Edoha.Domain.Models.Requests.Ticket;

namespace Edoha.Domain.Services
{
    public class TicketService : Service<Ticket>, ITicketService
    {
        private readonly ITicketbookRepository _ticketbookRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserService _userService;

        public TicketService(ITicketRepository repository, 
            ITicketbookRepository ticketbookRepository, 
            IUserService userService, 
            IRequestValidationContext requestValidationContext) 
            : base(repository, requestValidationContext)
        {
            _ticketbookRepository = ticketbookRepository;
            _ticketRepository = repository;
            _userService = userService;
        }

        public async Task InsertTicket(CreateTicketRequest ticketRequest)
        {
            var configurationTask = _ticketbookRepository.SelectTicketbookConfiguration(ticketRequest.IdTicketbook);

            var ticketTask = _ticketRepository.TicketExists(ticketRequest.IdTicketbook, ticketRequest.Number);

            var insertDonaterTask = _userService.InsertUserInformation(ticketRequest.TicketDonater.Name, ticketRequest.TicketDonater.Phone);

            await Task.WhenAll(configurationTask, ticketTask, insertDonaterTask);
            await ValidateTicket(ticketTask.Result, ticketRequest.Number, configurationTask.Result);
            var idDonater = insertDonaterTask.Result;

            DateTime soldDate = ticketRequest.SoldDate ?? DateTime.Now;

            CreateTicketDTO dto = new CreateTicketDTO
            {
                IdTicketbook = ticketRequest.IdTicketbook,
                IdDonater = idDonater,
                Number = ticketRequest.Number,
                SoldDate = soldDate
            };

            await Insert(dto);
        }

        public async Task<Ticket> SelectTicketById(Guid id)
        {
            return await _repository.SelectById(id);
        }

        public async Task<IEnumerable<Ticket>> SelectAllTickets()
        {
            return await _repository.SelectAll();
        }

        public async Task UpdateTicketById(UpdateTicketDTO dto)
        {
            await this.Update(dto);
        }

        public async Task DeleteTicketById(Guid id)
        {
            await this.DeleteById(id);
        }

        private async Task ValidateTicket(bool ticketExists, int ticketNumber, TicketbookConfigurationDTO? ticketbookConfiguration)
        {
            var vDuplicateTask = ValidateDuplicate(ticketExists);

            if(ticketbookConfiguration is not null)
            {
                var vNumberTask = ValidateNumber(ticketNumber, ticketbookConfiguration.Number, ticketbookConfiguration.NumTicketsTicketbook);

                if (ticketbookConfiguration.DoubleChance)
                {
                    var vDoubleChanceTask = ValidateDoubleChance(ticketNumber, ticketbookConfiguration.Number, ticketbookConfiguration.NumTicketsTicketbook, ticketbookConfiguration.NumTicketbooks);
                    await Task.WhenAll(vDuplicateTask, vNumberTask, vDoubleChanceTask);
                }
                else
                {
                    await Task.WhenAll(vDuplicateTask, vNumberTask);
                }
            }
            else
            {
                await _requestValidationContext.AddError("send", "Não foi possível encontrar o talão associado a esse número.");
                await vDuplicateTask;
            }
        }

        private async Task ValidateDuplicate(bool ticketExists)
        {
            if (ticketExists)
            {
                await _requestValidationContext.AddError("number", "O número já foi comprado.");
            }
        }

        private async Task ValidateNumber(int ticketNumber, int ticketbookNumber, int numTicketsTicketbook)
        {
            int minNumber = (ticketbookNumber * numTicketsTicketbook) - numTicketsTicketbook + 1;
            int maxNumber = ticketbookNumber * numTicketsTicketbook;

            if(ticketNumber > maxNumber || ticketNumber < minNumber)
            {
                await _requestValidationContext.AddError("number", @"O número é inválido. O talão só pode conter números entre {minValue} e {maxValue}");
            }
        }

        private async Task ValidateDoubleChance(int ticketNumber, int ticketbookNumber, int numTicketsTicketbook, int numTicketbooks)
        {
            int doubleChanceIncrement = numTicketsTicketbook * numTicketbooks;
            int minNumber = (ticketbookNumber * numTicketsTicketbook) - numTicketsTicketbook + 1;
            minNumber += doubleChanceIncrement;
            int maxNumber = ticketbookNumber * numTicketsTicketbook;
            maxNumber += doubleChanceIncrement;
            ticketNumber += doubleChanceIncrement;

            if (ticketNumber > maxNumber || ticketNumber < minNumber)
            {
                await _requestValidationContext.AddError("number", @"O número é inválido. O talão só pode conter números entre {minValue} e {maxValue} para os casos de bilhete de segunda chance");
            }
        }
    }
}