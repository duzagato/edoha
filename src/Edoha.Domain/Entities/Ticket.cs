using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Entities
{
    internal class Ticket
    {
        public int IdTicket { get; set; } 
        public int IdTicketbook { get; set; }
        public int IdDonater { get; set; }
        public int NumberTicket { get; set; }
        public DateTime CreatedDateTicket { get; set; }
        public DateTime? SoldDateTicket { get; set; }
    }
}
