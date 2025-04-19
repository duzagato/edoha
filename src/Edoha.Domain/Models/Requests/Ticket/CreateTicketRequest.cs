using Edoha.Shared.Annotations.Numerical;
using Edoha.Domain.Models.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Models.Requests.Ticket
{
    public class CreateTicketRequest : Request
    {
        [Required]
        public int IdTicketbook { get; set; }

        public int? IdDonater { get; set; }

        [Required]
        [Min(1)]
        public int NumberTicket { get; set; }

        public DateTime? SoldDateTicket { get; set; }
    }
}
