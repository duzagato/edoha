using Edoha.Shared.Annotations.Numerical;
using Edoha.Domain.Models.DTOs.Lottery;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Models.DTOs.Lottery
{
    public class UpdateLotteryDTO : DTO
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Min(1)]
        public int NumTicketsTicketbook { get; set; }

        [Required]
        [Min(1)]
        public int NumTicketbooks { get; set; }

        [Required]
        [Min(1)]
        public decimal PriceTicket { get; set; }

        [Required]
        public bool DoubleChance { get; set; }
    }
}
