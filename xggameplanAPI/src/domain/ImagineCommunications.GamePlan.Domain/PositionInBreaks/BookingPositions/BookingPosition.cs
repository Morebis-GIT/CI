namespace ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositions
{
    public class BookingPosition
    {
        public static readonly int NoDefaultPosition = -1;

        public int Id { get; set; }
        public int Position { get; set; }
        public string Abbreviation { get; set; }
        public int BookingOrder { get; set; }
    }
}
