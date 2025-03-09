using Edoha.Domain.Annotations.Numerical;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.DTOs.Lottery
{
    public class CreateLotteryDTO
    {
        [Required]
        public string NameLottery { get; set; }

        [Required]
        [Min(1)]
        public int NumTicketsTicketbookLottery { get; set; }

        [Required]
        [Min(1)]
        public int NumTicketbooksLottery { get; set; }

        [Required]
        [Min(1)]
        public decimal PriceTicketLottery { get; set; }

        [Required]
        public bool DoubleChanceLottery { get; set; }
    }
}
