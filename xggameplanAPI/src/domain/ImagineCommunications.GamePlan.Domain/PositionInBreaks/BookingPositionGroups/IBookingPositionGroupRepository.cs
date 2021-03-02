using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups.Objects;

namespace ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositionGroups
{
    public interface IBookingPositionGroupRepository
    {
        void Add(BookingPositionGroup bookingPositionGroup);

        void AddRange(IEnumerable<BookingPositionGroup> bookingPositionGroups);

        void Delete(int id);

        void DeleteRangeByGroupId(IEnumerable<int> ids);

        BookingPositionGroup Get(int id);

        IEnumerable<BookingPositionGroup> GetAll();

        IEnumerable<BookingPositionGroup> GetByGroupIds(IEnumerable<int> groupIds);

        void SaveChanges();

        void Update(BookingPositionGroup bookingPositionGroup);
    }
}
