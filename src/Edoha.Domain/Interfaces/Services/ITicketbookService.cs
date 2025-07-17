using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Repositories;
using Edoha.Domain.Models.DTOs.Ticketbook;

namespace Edoha.Domain.Interfaces.Services
{
    public interface ITicketbookService : IService<Ticketbook>
    {
        public Task InsertTicketbook(CreateTicketbookDTO dto);

        public Task<Ticketbook> SelectTicketbookById(Guid id);

        public Task<IEnumerable<Ticketbook>> SelectAllTicketbooks();

        public Task UpdateTicketbookById(UpdateTicketbookDTO dto);

        public Task DeleteTicketbookById(Guid id);
    }
}
