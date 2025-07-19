using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Context;
using Edoha.Domain.Interfaces.Repositories;
using Edoha.Domain.Interfaces.Services;
using Edoha.Domain.Models.DTOs.StatusTicketbook;
using Edoha.Shared.Helpers;
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

        public StatusTicketbookService(IStatusTicketbookRepository repository,
            IRequestValidationContext requestValidationContext
            ) : base(repository, requestValidationContext) { }

        public async Task InsertStatusTicketbook(CreateStatusTicketbookDTO dto)
        {
            await this.Insert(dto);
        }

        public async Task<StatusTicketbook> SelectStatusTicketbookById(Guid id)
        {
            return await _repository.SelectById(id);
        }

        public async Task<IEnumerable<StatusTicketbook>> SelectAllStatusTicketbooks()
        {
            return await _repository.SelectAll();
        }

        public async Task UpdateStatusTicketbookById(UpdateStatusTicketbookDTO dto)
        {
            await this.Update(dto);
        }

        public async Task DeleteStatusTicketbookById(Guid id)
        {
            await this.DeleteById(id);
        }
    }
}
