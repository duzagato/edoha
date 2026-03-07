using LotteryEntity = Edoha.Domain.Entities.Lottery;
using UserEntity = Edoha.Domain.Entities.User;
using StatusTicketbookEntity = Edoha.Domain.Entities.StatusTicketbook;

namespace Edoha.Domain.Models.DTOs.Ticketbook
{
    public class TicketbookResponse
    {
        public Guid Id { get; set; }
        public LotteryEntity Lottery { get; set; }
        public UserEntity? Owner { get; set; }
        public UserEntity? Holder { get; set; }
        public StatusTicketbookEntity StatusTicketbook { get; set; }
        public int Number { get; set; }
        public DateTime? WithdrawnDate { get; set; }
        public DateTime? DevolutionDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserEntity? CreatedBy { get; set; }
    }
}
