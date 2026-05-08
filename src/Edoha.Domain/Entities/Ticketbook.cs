using Edoha.Domain.Annotations.Numerical;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Edoha.Domain.Entities
{
    [Table("ticketbook", Schema = "lottery")]
    public class Ticketbook : Entity
    {
        [Required]
        public Guid IdLottery { get; set; }

        [Required]
        public Owner TicketbookOwner { get; set; }

        public Holder? TicketbookHolder { get; set; }


        [Required]
        public int IdStatusTicketbook { get; set; }

        [Required]
        [Min(1)]
        public int Number { get; set; }
        public DateTime? WithdrawnDate { get; set; }
        public DateTime? DevolutionDate { get; set; }
        public IList<Ticket>? Tickets { get; set; }
    }

    public class Holder
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
     }

     public class Owner
     {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
     }
}