using Edoha.Shared.Helpers;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Repositories;
using Edoha.Domain.Interfaces.Services;
using Edoha.Domain.Models.DTOs.Ticketbook;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Edoha.Domain.Interfaces.Context;

namespace Edoha.Domain.Services
{
    public class TicketbookService : Service<Ticketbook>, ITicketbookService
    {
        private readonly IStatusTicketbookRepository _statusTicketbookRepository;
        private readonly ILotteryRepository _lotteryRepository;

        public TicketbookService(ITicketbookRepository repository, IStatusTicketbookRepository statusTicketbookRepository, ILotteryRepository lotteryRepository, IRequestValidationContext requestValidationContex) : base(repository, requestValidationContex) 
        {
            _statusTicketbookRepository = statusTicketbookRepository;
            _lotteryRepository = lotteryRepository;
        }

        public async Task InsertTicketbook(CreateTicketbookDTO dto)
        {
            await _lotteryRepository.IdExists(dto.IdLottery);
            await _statusTicketbookRepository.IdExists(dto.IdStatusTicketbook);
            await this.Insert(dto);
        }

        public async Task<Ticketbook> SelectTicketbookById(Guid id)
        {
            return await _repository.SelectById(id);
        }

        public async Task<IEnumerable<Ticketbook>> SelectAllTicketbooks()
        {
            return await _repository.SelectAll();
        }

        public async Task UpdateTicketbookById(UpdateTicketbookDTO dto)
        {
            await _statusTicketbookRepository.IdExists(dto.IdStatusTicketbook);
            await this.Update(dto);
        }

        public async Task DeleteTicketbookById(Guid id)
        {
            await this.DeleteById(id);
        }
    }
}
