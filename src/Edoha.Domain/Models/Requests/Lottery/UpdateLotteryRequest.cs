using Edoha.Shared.Annotations.Numerical;
using Edoha.Domain.Models.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Models.Requests.Lottery
{
    public class UpdateLotteryRequest : Request
    {
        [Required]
        public int IdLottery { get; set; }

        [Required]
        public string? NameLottery { get; set; }

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
