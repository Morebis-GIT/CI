using System.Collections.Generic;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.BookingPositionGroup;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.BookingPositionGroup
{
    public class BulkBookingPositionGroupDeleted : IBulkBookingPositionGroupDeleted
    {
        public BulkBookingPositionGroupDeleted(IEnumerable<BookingPositionGroupDeleted> data) => Data = data;

        public IEnumerable<IBookingPositionGroupDeleted> Data { get; }
    }
}
