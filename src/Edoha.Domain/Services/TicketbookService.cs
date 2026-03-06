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
        private readonly IUserRepository _userRepository;

        public TicketbookService(ITicketbookRepository repository, IStatusTicketbookRepository statusTicketbookRepository, ILotteryRepository lotteryRepository, IUserRepository userRepository, IRequestValidationContext requestValidationContex) : base(repository, requestValidationContex) 
        {
            _statusTicketbookRepository = statusTicketbookRepository;
            _lotteryRepository = lotteryRepository;
            _userRepository = userRepository;
        }

        public async Task InsertTicketbook(CreateTicketbookDTO dto)
        {
            await _lotteryRepository.IdExists(dto.IdLottery);
            await _statusTicketbookRepository.IdExists(dto.IdStatusTicketbook);
            await this.Insert(dto);
        }

        public async Task<TicketbookResponse> SelectTicketbookById(Guid id)
        {
            var ticketbook = await _repository.SelectById(id);
            return await BuildResponse(ticketbook);
        }

        public async Task<IEnumerable<TicketbookResponse>> SelectAllTicketbooks()
        {
            var ticketbooks = await _repository.SelectAll();
            var responses = new List<TicketbookResponse>();
            foreach (var ticketbook in ticketbooks)
            {
                responses.Add(await BuildResponse(ticketbook));
            }
            return responses;
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

        private async Task<TicketbookResponse> BuildResponse(Ticketbook ticketbook)
        {
            var lottery = await _lotteryRepository.SelectById(ticketbook.IdLottery);
            var statusTicketbook = await _statusTicketbookRepository.SelectById(ticketbook.IdStatusTicketbook);

            User? owner = null;
            if (ticketbook.IdOwner.HasValue)
                owner = await _userRepository.SelectById(ticketbook.IdOwner.Value);

            User? holder = null;
            if (ticketbook.IdHolder.HasValue)
                holder = await _userRepository.SelectById(ticketbook.IdHolder.Value);

            return new TicketbookResponse
            {
                Id = ticketbook.Id,
                CreateAt = ticketbook.CreateAt,
                CreateBy = ticketbook.CreateBy,
                Number = ticketbook.Number,
                WithdrawnDate = ticketbook.WithdrawnDate,
                DevolutionDate = ticketbook.DevolutionDate,
                Lottery = lottery,
                StatusTicketbook = statusTicketbook,
                Owner = owner,
                Holder = holder
            };
        }
    }
}
