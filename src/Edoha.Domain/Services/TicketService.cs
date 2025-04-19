using Edoha.Shared.Helpers;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Repositories;
using Edoha.Domain.Interfaces.Services;
using Edoha.Domain.Models.Requests.Ticket;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Services
{
    public class TicketService : Service<Ticket>, ITicketService
    {
        private readonly ITicketbookRepository _ticketbookRepository;

        public TicketService(ITicketRepository repository, ITicketbookRepository ticketbookRepository) : base(repository) 
        {
            _ticketbookRepository = ticketbookRepository;
        }

        public async Task InsertTicket(CreateTicketRequest request)
        {
            await _ticketbookRepository.ValidateId(request.IdTicketbook);

            await this.Insert(request);
        }

        public async Task<Ticket> SelectTicketById(int id)
        {
            return await _repository.SelectById(id);
        }

        public async Task<IEnumerable<Ticket>> SelectAllTickets()
        {
            return await _repository.SelectAll();
        }

        public async Task UpdateTicketById(UpdateTicketRequest request)
        {
            await this.Update(request);
        }

        public async Task DeleteTicketById(int id)
        {
            await this.DeleteById(id);
        }
    }
}
