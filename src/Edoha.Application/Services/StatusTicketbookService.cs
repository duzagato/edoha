using Edoha.Application.Helpers;
using Edoha.Domain.DTOs.StatusTicketbook;
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
    public class StatusTicketbookService : Service<StatusTicketbook>
    {

        public StatusTicketbookService(IStatusTicketbookRepository repository) : base(repository) { }

        public async Task InsertStatusTicketbook(CreateStatusTicketbookDTO dto)
        {
            await this.Insert(dto);
        }

        public async Task<StatusTicketbook> SelectStatusTicketbookById(int id)
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

        public async Task DeleteStatusTicketbookById(int id)
        {
            await this.DeleteById(id);
        }
    }
}
