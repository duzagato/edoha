using System.ComponentModel.DataAnnotations.Schema;

namespace Edoha.Domain.Entities
{
    [Table("Ticket", Schema = "RIFAS")]
    public class Ticket: Entity
    {
        public int IdTicketbook { get; set; }
        public int IdDonater { get; set; } // id User
        public int Number { get; set; }
        public DateTime? SoldDate { get; set; }
    }
}
