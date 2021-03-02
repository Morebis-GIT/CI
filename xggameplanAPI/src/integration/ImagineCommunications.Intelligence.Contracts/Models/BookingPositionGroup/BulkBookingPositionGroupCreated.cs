using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.BookingPositionGroup;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.BookingPositionGroup
{
    public class BulkBookingPositionGroupCreated : IBulkBookingPositionGroupCreated
    {
        public BulkBookingPositionGroupCreated(IEnumerable<BookingPositionGroupCreated> data) => Data = data;

        public IEnumerable<IBookingPositionGroupCreated> Data { get; }
    }
}
