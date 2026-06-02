using Edoha.Domain.Entities;
using Edoha.Domain.Exceptions;
using Edoha.Domain.Interfaces.Domain.Services;
using Edoha.Domain.Interfaces.Infraestructure.Context;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Models.DTOs.Ticket;
using Edoha.Domain.Models.DTOs.Ticketbook;
using Edoha.Domain.Models.Requests.Ticket;
using Microsoft.Extensions.Logging;

namespace Edoha.Domain.Services
{
    public class TicketService : Service<Ticket>, ITicketService
    {
        private readonly ITicketbookRepository _ticketbookRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserService _userService;
        private readonly ILogger<TicketService> _logger;

        public TicketService(ITicketRepository repository, 
            ITicketbookRepository ticketbookRepository, 
            IUserService userService,
            ILogger<TicketService> logger, 
            IRequestValidationContext requestValidationContext) 
            : base(repository, requestValidationContext, logger)
        {
            _ticketbookRepository = ticketbookRepository;
            _ticketRepository = repository;
            _userService = userService;
            _logger = logger;
        }

        public async Task InsertTicket(Guid idTicketbook, CreateTicketRequest ticketRequest)
        {
            _logger.LogInformation("Iniciando método InsertTicket");
            _logger.LogInformation("Parâmetros recebidos - idTicketbook: {IdTicketbook}, ticketRequest: {@TicketRequest}", idTicketbook, ticketRequest);

            _logger.LogInformation("Verificando se ticketbook {IdTicketbook} existe", idTicketbook);
            await _ticketbookRepository.IdExists(idTicketbook);

            _logger.LogInformation("Buscando configuração do ticketbook {IdTicketbook}", idTicketbook);
            var ticketbookConfiguration = await _ticketbookRepository.SelectTicketbookConfiguration(idTicketbook);
            _logger.LogInformation("Configuração do ticketbook recebida: {@TicketbookConfiguration}", ticketbookConfiguration);

            _logger.LogInformation("Verificando se ticket com número {Number} já existe no ticketbook {IdTicketbook}", ticketRequest.Number, idTicketbook);
            var ticketExists = await _ticketRepository.TicketExists(idTicketbook, ticketRequest.Number);
            _logger.LogInformation("Ticket existe: {TicketExists}", ticketExists);

            _logger.LogInformation("Iniciando validação do ticket");
            await ValidateTicket(ticketExists, ticketRequest.Number, ticketbookConfiguration);

            if (_requestValidationContext.GetErrors().Count > 0)
            {
                _logger.LogWarning("Validação do ticket falhou. Erros: {@Errors}", _requestValidationContext.GetErrors());
                throw new RequestValidationException(_requestValidationContext.GetErrors());
            }

            _logger.LogInformation("Inserindo informações do doador - Nome: {Name}, Telefone: {Phone}", ticketRequest.DonaterName, ticketRequest.DonaterPhone);
            var idDonater = await _userService.InsertUserInformation(ticketRequest.DonaterName, ticketRequest.DonaterPhone);
            _logger.LogInformation("Doador inserido/recuperado com Id: {IdDonater}", idDonater);

            DateTime soldDate = ticketRequest.SoldDate ?? DateTime.Now;
            _logger.LogInformation("Data de venda definida: {SoldDate}", soldDate);

            CreateTicketDTO dto = new CreateTicketDTO
            {
                IdTicketbook = idTicketbook,
                IdDonater = idDonater,
                Number = ticketRequest.Number,
                SoldDate = soldDate
            };

            _logger.LogInformation("DTO criado para inserção: {@Dto}", dto);
            await Insert(dto);
            _logger.LogInformation("Ticket inserido com sucesso no ticketbook {IdTicketbook}", idTicketbook);
        }

        public async Task<Ticket> SelectTicketById(Guid idTicketbook, Guid id)
        {
            _logger.LogInformation("Iniciando método SelectTicketById");
            _logger.LogInformation("Parâmetros recebidos - idTicketbook: {IdTicketbook}, id: {Id}", idTicketbook, id);

            _logger.LogInformation("Verificando se ticketbook {IdTicketbook} existe", idTicketbook);
            await _ticketbookRepository.IdExists(idTicketbook);

            _logger.LogInformation("Buscando ticket {Id} no ticketbook {IdTicketbook}", id, idTicketbook);
            var ticket = await _ticketRepository.SelectByIdAndTicketbook(id, idTicketbook);

            _logger.LogInformation("Ticket encontrado: {@Ticket}. Retornando resultado", ticket);
            return ticket;
        }

        public async Task<IEnumerable<Ticket>> SelectAllTickets(Guid idTicketbook)
        {
            _logger.LogInformation("Iniciando método SelectAllTickets");
            _logger.LogInformation("Parâmetros recebidos - idTicketbook: {IdTicketbook}", idTicketbook);

            _logger.LogInformation("Verificando se ticketbook {IdTicketbook} existe", idTicketbook);
            await _ticketbookRepository.IdExists(idTicketbook);

            _logger.LogInformation("Buscando todos os tickets do ticketbook {IdTicketbook}", idTicketbook);
            var tickets = await _ticketRepository.SelectAllByTicketbook(idTicketbook);

            _logger.LogInformation("Total de {Count} tickets encontrados para o ticketbook {IdTicketbook}. Retornando resultado", tickets.Count(), idTicketbook);
            return tickets;
        }

        public async Task UpdateTicketById(Guid idTicketbook, UpdateTicketDTO dto)
        {
            _logger.LogInformation("Iniciando método UpdateTicketById");
            _logger.LogInformation("Parâmetros recebidos - idTicketbook: {IdTicketbook}, dto: {@Dto}", idTicketbook, dto);

            _logger.LogInformation("Verificando se ticketbook {IdTicketbook} existe", idTicketbook);
            await _ticketbookRepository.IdExists(idTicketbook);

            _logger.LogInformation("Atualizando ticket no ticketbook {IdTicketbook}", idTicketbook);
            await this.Update(dto);

            _logger.LogInformation("Ticket atualizado com sucesso no ticketbook {IdTicketbook}", idTicketbook);
        }

        public async Task DeleteTicketById(Guid idTicketbook, Guid id)
        {
            _logger.LogInformation("Iniciando método DeleteTicketById");
            _logger.LogInformation("Parâmetros recebidos - idTicketbook: {IdTicketbook}, id: {Id}", idTicketbook, id);

            _logger.LogInformation("Verificando se ticketbook {IdTicketbook} existe", idTicketbook);
            await _ticketbookRepository.IdExists(idTicketbook);

            _logger.LogInformation("Deletando ticket {Id} do ticketbook {IdTicketbook}", id, idTicketbook);
            await this.DeleteById(id);

            _logger.LogInformation("Ticket {Id} deletado com sucesso do ticketbook {IdTicketbook}", id, idTicketbook);
        }

        private async Task ValidateTicket(bool ticketExists, int ticketNumber, TicketbookConfigurationDTO? ticketbookConfiguration)
        {
            _logger.LogInformation("Iniciando método ValidateTicket");
            _logger.LogInformation("Parâmetros recebidos - ticketExists: {TicketExists}, ticketNumber: {TicketNumber}, ticketbookConfiguration: {@TicketbookConfiguration}", ticketExists, ticketNumber, ticketbookConfiguration);

            var vDuplicateTask = ValidateDuplicate(ticketExists);

            if(ticketbookConfiguration is not null)
            {
                _logger.LogInformation("Configuração do ticketbook encontrada. Validando número do ticket");
                var vNumberTask = ValidateNumber(ticketNumber, ticketbookConfiguration.Number, ticketbookConfiguration.NumTicketsTicketbook);

                if (ticketbookConfiguration.DoubleChance)
                {
                    _logger.LogInformation("DoubleChance está habilitado. Validando segunda chance");
                    var vDoubleChanceTask = ValidateDoubleChance(ticketNumber, ticketbookConfiguration.Number, ticketbookConfiguration.NumTicketsTicketbook, ticketbookConfiguration.NumTicketbooks);
                    await Task.WhenAll(vDuplicateTask, vNumberTask, vDoubleChanceTask);
                }
                else
                {
                    _logger.LogInformation("DoubleChance não está habilitado");
                    await Task.WhenAll(vDuplicateTask, vNumberTask);
                }
            }
            else
            {
                _logger.LogError("Configuração do ticketbook não encontrada");
                await _requestValidationContext.AddError("send", "Não foi possível encontrar o talão associado a esse número.");
                await vDuplicateTask;
            }

            _logger.LogInformation("Método ValidateTicket finalizado");
        }

        private async Task ValidateDuplicate(bool ticketExists)
        {
            _logger.LogInformation("Iniciando método ValidateDuplicate");
            _logger.LogInformation("Parâmetros recebidos - ticketExists: {TicketExists}", ticketExists);

            if (ticketExists)
            {
                _logger.LogWarning("Ticket já existe. Adicionando erro de validação");
                await _requestValidationContext.AddError("number", "O número já foi comprado.");
            }
            else
            {
                _logger.LogInformation("Ticket não existe. Validação de duplicidade passou");
            }

            _logger.LogInformation("Método ValidateDuplicate finalizado");
        }

        private async Task ValidateNumber(int ticketNumber, int ticketbookNumber, int numTicketsTicketbook)
        {
            _logger.LogInformation("Iniciando método ValidateNumber");
            _logger.LogInformation("Parâmetros recebidos - ticketNumber: {TicketNumber}, ticketbookNumber: {TicketbookNumber}, numTicketsTicketbook: {NumTicketsTicketbook}", ticketNumber, ticketbookNumber, numTicketsTicketbook);

            int minNumber = (ticketbookNumber * numTicketsTicketbook) - numTicketsTicketbook + 1;
            int maxNumber = ticketbookNumber * numTicketsTicketbook;

            _logger.LogInformation("Faixa de números válidos calculada - minNumber: {MinNumber}, maxNumber: {MaxNumber}", minNumber, maxNumber);

            if(ticketNumber > maxNumber || ticketNumber < minNumber)
            {
                _logger.LogWarning("Número do ticket {TicketNumber} está fora da faixa válida [{MinNumber}-{MaxNumber}]. Adicionando erro de validação", ticketNumber, minNumber, maxNumber);
                await _requestValidationContext.AddError("number", @"O número é inválido. O talão só pode conter números entre {minValue} e {maxValue}");
            }
            else
            {
                _logger.LogInformation("Número do ticket {TicketNumber} está dentro da faixa válida", ticketNumber);
            }

            _logger.LogInformation("Método ValidateNumber finalizado");
        }

        private async Task ValidateDoubleChance(int ticketNumber, int ticketbookNumber, int numTicketsTicketbook, int numTicketbooks)
        {
            _logger.LogInformation("Iniciando método ValidateDoubleChance");
            _logger.LogInformation("Parâmetros recebidos - ticketNumber: {TicketNumber}, ticketbookNumber: {TicketbookNumber}, numTicketsTicketbook: {NumTicketsTicketbook}, numTicketbooks: {NumTicketbooks}", ticketNumber, ticketbookNumber, numTicketsTicketbook, numTicketbooks);

            int doubleChanceIncrement = numTicketsTicketbook * numTicketbooks;
            int minNumber = (ticketbookNumber * numTicketsTicketbook) - numTicketsTicketbook + 1;
            minNumber += doubleChanceIncrement;
            int maxNumber = ticketbookNumber * numTicketsTicketbook;
            maxNumber += doubleChanceIncrement;
            ticketNumber += doubleChanceIncrement;

            _logger.LogInformation("Faixa de números válidos para segunda chance calculada - minNumber: {MinNumber}, maxNumber: {MaxNumber}, ticketNumber ajustado: {AdjustedTicketNumber}", minNumber, maxNumber, ticketNumber);

            if (ticketNumber > maxNumber || ticketNumber < minNumber)
            {
                _logger.LogWarning("Número do ticket {TicketNumber} está fora da faixa válida para segunda chance [{MinNumber}-{MaxNumber}]. Adicionando erro de validação", ticketNumber, minNumber, maxNumber);
                await _requestValidationContext.AddError("number", $"O número é inválido. O talão só pode conter números entre {minNumber} e {maxNumber} para os casos de bilhete de segunda chance");
            }
            else
            {
                _logger.LogInformation("Número do ticket {TicketNumber} está dentro da faixa válida para segunda chance", ticketNumber);
            }

            _logger.LogInformation("Método ValidateDoubleChance finalizado");
        }
    }
}