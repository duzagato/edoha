using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.DTOs.Lottery
{
    public class CreateLotteryDTO
    {
        public string NameLottery { get; set; }
        public int NumTicketsTicketbookLottery { get; set; }
        public int NumTicketbooksLottery { get; set; }
        public decimal PriceTicketLottery { get; set; }
        public bool DoubleChanceLottery { get; set; }
    }
}
