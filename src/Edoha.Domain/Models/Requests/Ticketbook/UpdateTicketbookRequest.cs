using Edoha.Shared.Annotations.Numerical;
using Edoha.Domain.Models.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Models.Requests.Ticketbook
{
    public class UpdateTicketbookRequest : Request
    {
        public int? IdOwner { get; set; }
        public int? IdHolder { get; set; }

        [Min(1)]
        public int IdStatusTicketbook { get; set; }
        public DateTime? WithdrawnDateTicketbook { get; set; }
        public DateTime? DevolutionDateTicketbook { get; set; }
    }
}
