using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Repositories;
using Edoha.Domain.Models.Requests.Ticketbook;

namespace Edoha.Domain.Interfaces.Services
{
    public interface ITicketbookService : IService<Ticketbook>
    {
        public Task InsertTicketbook(CreateTicketbookRequest request);

        public Task<Ticketbook> SelectTicketbookById(int id);

        public Task<IEnumerable<Ticketbook>> SelectAllTicketbooks();

        public Task UpdateTicketbookById(UpdateTicketbookRequest request);

        public Task DeleteTicketbookById(int id);
    }
}
