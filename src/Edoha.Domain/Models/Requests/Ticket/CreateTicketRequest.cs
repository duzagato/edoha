using Edoha.Domain.Annotations.Numerical;
using System.ComponentModel.DataAnnotations;

namespace Edoha.Domain.Models.Requests.Ticket
{
    public class CreateTicketRequest
    {
        public string DonaterName { get; set; }

        public string DonaterPhone { get; set; }

        [Required]
        [Min(1)]
        public int Number { get; set; }

        public DateTime? SoldDate { get; set; }
    }
}
