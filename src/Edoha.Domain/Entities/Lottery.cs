using System.ComponentModel.DataAnnotations.Schema;

namespace Edoha.Domain.Entities
{
    [Table("Lottery", Schema = "RIFAS")]
    public class Lottery: Entity
    {
        public string Name { get; set; }
        public int NumTicketsTicketbook{ get; set; }
        public int NumTicketbooks { get; set; }
        public decimal PriceTicket { get; set; }
        public bool DoubleChance { get; set; }
    }
}
