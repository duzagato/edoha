using Edoha.Domain.Annotations.Numerical;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.DTOs.Ticketbook
{
    public class CreateTicketbookDTO : DTO
    {
        [Required]
        public int IdLottery { get; set; }
        public int? IdOwner { get; set; }
        public int? IdHolder { get; set; }

        [Required]
        public int IdStatusTicketbook { get; set; }

        [Required]
        [Min(1)]
        public int NumberTicketbook { get; set; }
        public DateTime? WithdrawnDateTicketbook { get; set; }
        public DateTime? DevolutionDateTicketbook { get; set; }
    }
}
