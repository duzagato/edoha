using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Entities
{
    [Table("Ticketbook", Schema = "RIFAS")]
    public class Ticketbook
    {
        public int IdTicketbook { get; set; }
        public int IdLottery { get; set; }
        public int? IdOwner { get; set; }
        public int? IdHolder { get; set; }
        public int IdStatusTicketbook { get; set; }
        public int NumberTicketbook { get; set; }
        public DateTime InsertionDateTicketbook { get; set; }
        public DateTime? WithdrawnDateTicketbook { get; set; }
        public DateTime? DevolutionDateTicketbook { get; set; }
    }
}
