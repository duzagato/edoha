using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edoha.Domain.Entities;
using Edoha.Domain.Models.DTOs.StatusTicketbook;

namespace Edoha.Domain.Interfaces.Services
{
    public interface IStatusTicketbookService : IService<StatusTicketbook>
    {
        public Task InsertStatusTicketbook(CreateStatusTicketbookDTO dto);

        public Task<StatusTicketbook> SelectStatusTicketbookById(Guid id);

        public Task<IEnumerable<StatusTicketbook>> SelectAllStatusTicketbooks();

        public Task UpdateStatusTicketbookById(UpdateStatusTicketbookDTO dto);

        public Task DeleteStatusTicketbookById(Guid id);
    }
}
