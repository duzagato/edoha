using Edoha.Shared.Helpers;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Repositories;
using Edoha.Domain.Interfaces.Services;
using Edoha.Domain.Models.Requests.StatusTicketbook;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Services
{
    public class StatusTicketbookService : Service<StatusTicketbook>, IStatusTicketbookService
    {

        public StatusTicketbookService(IStatusTicketbookRepository repository) : base(repository) { }

        public async Task InsertStatusTicketbook(CreateStatusTicketbookRequest request)
        {
            await this.Insert(request);
        }

        public async Task<StatusTicketbook> SelectStatusTicketbookById(int id)
        {
            return await _repository.SelectById(id);
        }

        public async Task<IEnumerable<StatusTicketbook>> SelectAllStatusTicketbooks()
        {
            return await _repository.SelectAll();
        }

        public async Task UpdateStatusTicketbookById(UpdateStatusTicketbookRequest request)
        {
            await this.Update(request);
        }

        public async Task DeleteStatusTicketbookById(int id)
        {
            await this.DeleteById(id);
        }
    }
}
