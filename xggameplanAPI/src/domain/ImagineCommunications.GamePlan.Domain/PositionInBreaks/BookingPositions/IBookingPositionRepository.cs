using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.PositionInBreaks.BookingPositions
{
    public interface IBookingPositionRepository
    {
        BookingPosition Get(int id);

        IEnumerable<BookingPosition> GetAll();

        BookingPosition GetByPosition(int position);

        IEnumerable<BookingPosition> GetByPositions(IEnumerable<int> positions);
    }
}
