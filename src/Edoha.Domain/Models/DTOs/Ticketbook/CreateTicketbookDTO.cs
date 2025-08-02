using Edoha.Domain.Annotations.Numerical;
using System.ComponentModel.DataAnnotations;

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
