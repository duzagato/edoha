using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Entities
{
    [Table("ticketbook", Schema = "lottery")]
    public class Ticketbook : Entity
    {
        public Guid IdLottery { get; set; }
        public Guid? IdOwner { get; set; }
        public Guid? IdHolder { get; set; }
        public Guid IdStatusTicketbook { get; set; }
        public int Number { get; set; }
        public DateTime? WithdrawnDate { get; set; }
        public DateTime? DevolutionDate { get; set; }
    }
}
