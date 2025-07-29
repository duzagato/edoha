using Edoha.Domain.Annotations.Numerical;
using System.ComponentModel.DataAnnotations;

namespace Edoha.Domain.Models.DTOs.Ticket
{
    public class CreateTicketDTO : DTO
    {
        [Required]
        public Guid IdTicketbook { get; set; }

        public Guid? IdDonater { get; set; }

        [Required]
        [Min(1)]
        public int Number { get; set; }

        public DateTime? SoldDate { get; set; }
    }
}
