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
    public class UpdateTicketRequest : Request
    {
        [Required]
        public int IdTicket { get; set; }
        public int? IdDonater { get; set; }

        public DateTime? SoldDateTicket { get; set; }
    }
}
