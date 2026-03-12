using Edoha.Domain.Annotations.Numerical;
using System.ComponentModel.DataAnnotations;

namespace Edoha.Domain.Models.Requests.Ticket
{
    public class CreateTicketRequest
    {
        [Required]
        public Guid IdTicketbook { get; set; }

        public Donater TicketDonater { get; set; }

        [Required]
        [Min(1)]
        public int Number { get; set; }

        public DateTime? SoldDate { get; set; }
    }

    public class Donater
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Phone { get; set; }
    }
}
