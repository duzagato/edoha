using System.ComponentModel.DataAnnotations.Schema;

namespace Edoha.Domain.Entities
{
    [Table("Ticketbook", Schema = "RIFAS")]
    public class Ticketbook: Entity
    {
        public int IdLottery { get; set; }
        public int? IdOwner { get; set; }
        public int? IdHolder { get; set; }
        public int IdStatus{ get; set; }
        public int Number { get; set; }
        public DateTime? WithdrawnDate { get; set; }
        public DateTime? DevolutionDate { get; set; }
    }
}
// IdOwner e IdHolder vem User