using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Repositories;
using Edoha.Domain.Models.Requests.Ticket;

namespace Edoha.Domain.Interfaces.Services
{
    public interface ITicketService : IService<Ticket>
    {
        public Task InsertTicket(CreateTicketRequest request);

        public Task<Ticket> SelectTicketById(int id);

        public Task<IEnumerable<Ticket>> SelectAllTickets();

        public Task UpdateTicketById(UpdateTicketRequest request);

        public Task DeleteTicketById(int id);
    }
}
