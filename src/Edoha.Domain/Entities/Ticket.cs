using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Entities
{
    [Table("ticket", Schema = "lottery")]
    public class Ticket
    {
        public Guid IdTicketbook { get; set; }
        public Guid IdDonater { get; set; }
        public int Number { get; set; }
        public DateTime? SoldDate { get; set; }
    }
}
