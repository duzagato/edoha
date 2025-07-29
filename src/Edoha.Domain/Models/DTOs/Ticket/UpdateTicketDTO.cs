using System.ComponentModel.DataAnnotations;

namespace Edoha.Domain.Models.DTOs.Ticket
{
    public class UpdateTicketDTO : DTO
    {
        [Required]
        public Guid Id{ get; set; }
        public Guid? IdDonater { get; set; }

        public DateTime? SoldDate { get; set; }
    }
}
