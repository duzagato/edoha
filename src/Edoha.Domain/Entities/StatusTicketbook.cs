using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edoha.Domain.Entities
{
    [Table("status_ticketbook", Schema = "lottery")]
    public class StatusTicketbook : Entity
    {
        [Required]
        public string Name { get; set; }
    }
}
