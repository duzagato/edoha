using Edoha.Domain.Entities;

namespace Edoha.Domain.Models.DTOs.Ticketbook
{
    public class TicketbookResponse
    {
        public Guid Id { get; set; }
        public DateTime CreateAt { get; set; }
        public Guid? CreateBy { get; set; }
        public int Number { get; set; }
        public DateTime? WithdrawnDate { get; set; }
        public DateTime? DevolutionDate { get; set; }
        public Entities.Lottery Lottery { get; set; }
        public Entities.User? Owner { get; set; }
        public Entities.User? Holder { get; set; }
        public Entities.StatusTicketbook StatusTicketbook { get; set; }
    }
}
