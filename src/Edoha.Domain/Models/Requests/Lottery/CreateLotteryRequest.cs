using System.ComponentModel.DataAnnotations;

namespace Edoha.Domain.Models.Requests.Lottery
{
    public class CreateLotteryRequest
    {
        [Required]
        public Guid IdInstitution { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int NumTicketsTicketbook { get; set; }

        [Required]
        public int NumTicketbooks { get; set; }

        [Required]
        public decimal PriceTicket { get; set; }

        [Required]
        public bool DoubleChance { get; set; }
    }
}
