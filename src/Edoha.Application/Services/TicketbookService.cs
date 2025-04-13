using Edoha.Application.Helpers;
using Edoha.Domain.DTOs.Ticketbook;
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
    public class TicketbookService : Service<Ticketbook>
    {
        private readonly IStatusTicketbookRepository _statusTicketbookRepository;
        private readonly ILotteryRepository _lotteryRepository;

        public TicketbookService(ITicketbookRepository repository, IStatusTicketbookRepository statusTicketbookRepository, ILotteryRepository lotteryRepository) : base(repository) 
        {
            _statusTicketbookRepository = statusTicketbookRepository;
            _lotteryRepository = lotteryRepository;
        }

        public async Task InsertTicketbook(CreateTicketbookDTO dto)
        {
            await _lotteryRepository.ValidateId(dto.IdLottery);
            await _statusTicketbookRepository.ValidateId(dto.IdStatusTicketbook);
            await this.Insert(dto);
        }

        public async Task<Ticketbook> SelectTicketbookById(int id)
        {
            return await _repository.SelectById(id);
        }

        public async Task<IEnumerable<Ticketbook>> SelectAllTicketbooks()
        {
            return await _repository.SelectAll();
        }

        public async Task UpdateTicketbookById(UpdateTicketbookDTO dto)
        {
            await _statusTicketbookRepository.ValidateId(dto.IdStatusTicketbook);
            await this.Update(dto);
        }

        public async Task DeleteTicketbookById(int id)
        {
            await this.DeleteById(id);
        }
    }
}
