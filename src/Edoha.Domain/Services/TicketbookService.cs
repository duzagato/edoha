using Edoha.Shared.Helpers;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces.Repositories;
using Edoha.Domain.Interfaces.Services;
using Edoha.Domain.Models.Requests.Ticketbook;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Services
{
    public class TicketbookService : Service<Ticketbook>, ITicketbookService
    {
        private readonly IStatusTicketbookRepository _statusTicketbookRepository;
        private readonly ILotteryRepository _lotteryRepository;

        public TicketbookService(ITicketbookRepository repository, IStatusTicketbookRepository statusTicketbookRepository, ILotteryRepository lotteryRepository) : base(repository) 
        {
            _statusTicketbookRepository = statusTicketbookRepository;
            _lotteryRepository = lotteryRepository;
        }

        public async Task InsertTicketbook(CreateTicketbookRequest request)
        {
            await _lotteryRepository.ValidateId(request.IdLottery);
            await _statusTicketbookRepository.ValidateId(request.IdStatusTicketbook);
            await this.Insert(request);
        }

        public async Task<Ticketbook> SelectTicketbookById(int id)
        {
            return await _repository.SelectById(id);
        }

        public async Task<IEnumerable<Ticketbook>> SelectAllTicketbooks()
        {
            return await _repository.SelectAll();
        }

        public async Task UpdateTicketbookById(UpdateTicketbookRequest request)
        {
            await _statusTicketbookRepository.ValidateId(request.IdStatusTicketbook);
            await this.Update(request);
        }

        public async Task DeleteTicketbookById(int id)
        {
            await this.DeleteById(id);
        }
    }
}
