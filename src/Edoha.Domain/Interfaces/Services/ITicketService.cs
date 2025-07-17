using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Repositories;
using Edoha.Domain.Models.DTOs.Ticket;

namespace Edoha.Domain.Interfaces.Services
{
    public interface ITicketService : IService<Ticket>
    {
        public Task InsertTicket(CreateTicketDTO dto);

        public Task<Ticket> SelectTicketById(Guid id);

        public Task<IEnumerable<Ticket>> SelectAllTickets();

        public Task UpdateTicketById(UpdateTicketDTO dto);

        public Task DeleteTicketById(Guid id);
    }
}
