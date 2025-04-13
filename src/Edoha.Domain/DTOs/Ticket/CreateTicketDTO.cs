using Edoha.Domain.Annotations.Numerical;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.DTOs.Ticket
{
    public class CreateTicketDTO : DTO
    {
        [Required]
        public int IdTicketbook {get; set;}

        public int? IdDonater {get; set;}

        [Required]
        [Min(1)]
        public int NumberTicket { get; set; }

        public DateTime? SoldDateTicket { get; set; }
    }
}
