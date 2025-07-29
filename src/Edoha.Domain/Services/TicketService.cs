using Edoha.Domain.Entities;
using Edoha.Domain.Models.DTOs.Ticket;
using Edoha.Domain.Interfaces.Infraestructure.Context;
using Edoha.Domain.Interfaces.Infraestructure.Repositories;
using Edoha.Domain.Interfaces.Domain.Services;

namespace Edoha.Domain.Services
{
    public class TicketService : Service<Ticket>, ITicketService
    {
        private readonly ITicketbookRepository _ticketbookRepository;

        public TicketService(ITicketRepository repository, 
            ITicketbookRepository ticketbookRepository, 
            IRequestValidationContext requestValidationContext) 
            : base(repository, requestValidationContext) 
        {
            _ticketbookRepository = ticketbookRepository;
        }

        public async Task InsertTicket(CreateTicketDTO dto)
        {
            await _ticketbookRepository.IdExists(dto.IdTicketbook);

            await this.Insert(dto);
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
    }
}
