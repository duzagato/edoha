using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edoha.Domain.Entities;
using Edoha.Domain.Models.Requests.StatusTicketbook;

namespace Edoha.Domain.Interfaces.Services
{
    public interface IStatusTicketbookService : IService<StatusTicketbook>
    {
        public Task InsertStatusTicketbook(CreateStatusTicketbookRequest request);

        public Task<StatusTicketbook> SelectStatusTicketbookById(int id);

        public Task<IEnumerable<StatusTicketbook>> SelectAllStatusTicketbooks();

        public Task UpdateStatusTicketbookById(UpdateStatusTicketbookRequest request);

        public Task DeleteStatusTicketbookById(int id);
    }
}
