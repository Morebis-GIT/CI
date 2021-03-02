using ImagineCommunications.BusClient.Abstraction.Interfaces;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.BookingPositionGroup
{
    public interface IBookingPositionGroupDeleted : IEvent
    {
        int Id { get; }
    }
}
