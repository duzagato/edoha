using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Edoha.Domain.Annotations.Numerical;

namespace Edoha.Domain.Entities
{
    [Table("lottery", Schema = "lottery")]
    public class Lottery : Entity
    {
        [Required]
        public Guid IdInstitution { get; set; }

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
