using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Entities
{
    [Table("Ticket", Schema = "RIFAS")]
    public class Ticket
    {
        public int IdTicket { get; set; } 
        public int IdTicketbook { get; set; }
        public int IdDonater { get; set; }
        public int NumberTicket { get; set; }
        public DateTime CreatedDateTicket { get; set; }
        public DateTime? SoldDateTicket { get; set; }
    }
}
