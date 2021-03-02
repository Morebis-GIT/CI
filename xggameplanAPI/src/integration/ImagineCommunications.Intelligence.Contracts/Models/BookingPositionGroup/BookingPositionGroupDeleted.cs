using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.BookingPositionGroup;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.BookingPositionGroup
{
    public class BookingPositionGroupDeleted : IBookingPositionGroupDeleted
    {
        public BookingPositionGroupDeleted(int id) => Id = id;

        public int Id { get; }
    }
}
