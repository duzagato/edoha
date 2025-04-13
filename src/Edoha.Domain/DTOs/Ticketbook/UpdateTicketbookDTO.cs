using Edoha.Domain.Annotations.Numerical;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.DTOs.Ticketbook
{
    public class UpdateTicketbookDTO : DTO
    {
        public int? IdOwner { get; set; }
        public int? IdHolder { get; set; }

        [Min(1)]
        public int IdStatusTicketbook { get; set; }
        public DateTime? WithdrawnDateTicketbook { get; set; }
        public DateTime? DevolutionDateTicketbook { get; set; }
    }
}
