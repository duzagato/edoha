using Edoha.Domain.Annotations.Numerical;
using System.ComponentModel.DataAnnotations;

namespace Edoha.Domain.Models.Requests.Ticketbook
{
    public class PostTicketbookRequest
    {
        public Holder? TicketbookHolder { get; set; }
        
        [Required]
        public Owner TicketbookOwner { get; set; }

        [Required]
        public int IdStatusTicketbook { get; set; }

        [Required]
        [Min(1)]
        public int Number { get; set; }
        public DateTime? WithdrawnDate { get; set; }
        public DateTime? DevolutionDate { get; set; }
    }

    public class Holder
    {
        public string Name { get; set; }
        public string Phone { get; set; }
    }

    public class Owner
    {
        public string Name { get; set; }
        public string Phone { get; set; }
    }
}