using Edoha.Application.Helpers;
using Edoha.Domain.DTOs;
using Edoha.Domain.DTOs.Ticket;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Application.Services
{
    public class TicketService : Service<Ticket>
    {
        private readonly ITicketbookRepository _ticketbookRepository;

        public TicketService(ITicketRepository repository, ITicketbookRepository ticketbookRepository) : base(repository) 
        {
            _ticketbookRepository = ticketbookRepository;
        }

        public async Task InsertTicket(CreateTicketDTO dto)
        {
            await _ticketbookRepository.ValidateId(dto.IdTicketbook);

            await this.Insert(dto);
        }

        public async Task<Ticket> SelectTicketById(int id)
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

        public async Task DeleteTicketById(int id)
        {
            await this.DeleteById(id);
        }
    }
}
