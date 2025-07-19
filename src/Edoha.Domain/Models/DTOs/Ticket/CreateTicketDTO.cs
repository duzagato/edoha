using Edoha.Shared.Annotations.Numerical;
using Edoha.Domain.Models.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Models.DTOs.Ticket
{
    public class CreateTicketDTO : DTO
    {
        [Required]
        public Guid IdTicketbook { get; set; }

        public Guid? IdDonater { get; set; }

        [Required]
        [Min(1)]
        public int Number { get; set; }

        public DateTime? SoldDate { get; set; }
    }
}
