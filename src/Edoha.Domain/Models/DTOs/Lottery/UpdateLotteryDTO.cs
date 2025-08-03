using Edoha.Domain.Annotations.Numerical;
using System.ComponentModel.DataAnnotations;

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
