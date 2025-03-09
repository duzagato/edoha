using Edoha.Application.Helpers;
using Edoha.Domain.DTOs.Lottery;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces;
using Edoha.Domain.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Application.Services
{
    public class LotteryService : ILotteryService
    {
        public readonly ILotteryRepository _lotteryRepository;

        public LotteryService(ILotteryRepository lotteryRepository)
        {
            _lotteryRepository = lotteryRepository;
        }
        public void CreateLottery(CreateLotteryDTO dto)
        {
            ExceptionHelper.ValidateDTO(dto);

            var lottery = new Lottery
            {
                NameLottery = dto.NameLottery,
                NumTicketsTicketbookLottery = dto.NumTicketsTicketbookLottery,
                NumTicketbooksLottery = dto.NumTicketbooksLottery,
                PriceTicketLottery = dto.PriceTicketLottery,
                DoubleChanceLottery = dto.DoubleChanceLottery,
                InsertionDateLottery = DateTime.Now
            };

            _lotteryRepository.Insert(lottery);
        }

        public async Task<Lottery> SelectById(int id)
        {
            return await _lotteryRepository.SelectById(id);
        }

        public async Task<IEnumerable<Lottery>> SelectAll()
        {
            return await _lotteryRepository.SelectAll();
        }

        public async Task UpdateLotteryById(UpdateLotteryDTO dto)
        {
            ExceptionHelper.ValidateDTO(dto);

            var lottery = await _lotteryRepository.ValidateId(dto.IdLottery);
            lottery.NameLottery = dto.NameLottery;
            lottery.NumTicketbooksLottery = dto.NumTicketbooksLottery;
            lottery.NumTicketsTicketbookLottery = dto.NumTicketsTicketbookLottery;
            lottery.PriceTicketLottery = dto.PriceTicketLottery;
            lottery.DoubleChanceLottery = dto.DoubleChanceLottery;

            await _lotteryRepository.Update(lottery);
        }

        public async Task DeleteById(int id)
        {
            await _lotteryRepository.ValidateId(id);
            await _lotteryRepository.DeleteById(id);
        }
    }
}
