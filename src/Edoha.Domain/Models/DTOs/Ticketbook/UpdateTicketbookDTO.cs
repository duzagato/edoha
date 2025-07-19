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
    public class UpdateTicketbookDTO : DTO
    {
        public Guid? IdOwner { get; set; }
        public Guid? IdHolder { get; set; }

        [Min(1)]
        public Guid IdStatusTicketbook { get; set; }
        public DateTime? WithdrawnDate { get; set; }
        public DateTime? DevolutionDate { get; set; }
    }
}
