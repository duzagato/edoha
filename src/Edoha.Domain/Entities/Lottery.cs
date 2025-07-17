using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Edoha.Shared.Annotations.Numerical;

namespace Edoha.Domain.Entities
{
    [Table("lottery", Schema = "lottery")]
    public class Lottery : Entity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public int NumTicketsTicketbook { get; set; }

        [Required]
        public int NumTicketbooks { get; set; }

        [Required]
        public decimal PriceTicket { get; set; }

        [Required]
        public bool DoubleChance { get; set; }
    }
}
