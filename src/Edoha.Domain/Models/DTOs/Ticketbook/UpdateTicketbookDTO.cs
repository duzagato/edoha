using Edoha.Domain.Annotations.Numerical;

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
