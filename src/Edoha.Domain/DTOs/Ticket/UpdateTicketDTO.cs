using Edoha.Domain.Annotations.Numerical;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.DTOs.Ticket
{
    public class UpdateTicketDTO : DTO
    {
        [Required]
        public int IdTicket { get; set; }
        public int? IdDonater { get; set; }

        public DateTime? SoldDateTicket { get; set; }
    }
}
