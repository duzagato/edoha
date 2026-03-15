namespace Edoha.Infraestructure.Helpers
{
    public static class StaticQueries
    {
        public const string SelectTicketbookByNumber = @"SELECT * FROM lottery.ticketbook WHERE id_lottery = @IdLottery AND number = @Number";
    }
}
