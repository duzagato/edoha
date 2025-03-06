using Edoha.Domain.DTOs.Lottery;
using Edoha.Domain.Entities;
using Edoha.Domain.Interfaces;
using Edoha.Domain.Services;
using System;
using System.Collections.Generic;
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
            if (string.IsNullOrEmpty(dto.NameLottery))
                throw new ArgumentException("O nome da loteria é obrigatório.");

            if (dto.PriceTicketLottery <= 0)
                throw new ArgumentException("O preço do ticket deve ser maior que zero.");

            var lottery = new Lottery
            {
                NameLottery = dto.NameLottery,
                NumTicketsTicketbookLottery = dto.NumTicketsTicketbookLottery,
                NumTicketbooksLottery = dto.NumTicketbooksLottery,
                PriceTicketLottery = dto.PriceTicketLottery,
                DoubleChanceLottery = dto.DoubleChanceLottery,
                InsertionDateLottery = DateTime.Now // Define a data de inserção
            };

            _lotteryRepository.Insert(lottery);
        }
    }
}
