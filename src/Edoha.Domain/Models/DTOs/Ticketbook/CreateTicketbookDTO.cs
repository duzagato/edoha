using Edoha.Shared.Annotations.Numerical;
using Edoha.Domain.Models.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Models.DTOs.Ticketbook
{
    public class CreateTicketbookDTO : DTO
    {
        [Required]
        public Guid IdLottery{ get; set; }
        public Guid? IdOwner { get; set; }
        public Guid? IdHolder { get; set; }

        [Required]
        public Guid IdStatusTicketbook { get; set; }

        [Required]
        [Min(1)]
        public int Number { get; set; }
        public DateTime? WithdrawnDate{ get; set; }
        public DateTime? DevolutionDate { get; set; }
    }
}
