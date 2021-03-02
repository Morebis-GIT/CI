using System;

namespace ImagineCommunications.Gameplan.Integration.Contracts.Models.Shared
{
    public class MultipartLength
    {
        public MultipartLength(TimeSpan length, int bookingPosition, int sequencing)
        {
            Length = length;
            BookingPosition = bookingPosition;
            Sequencing = sequencing;
        }

        public TimeSpan Length { get; }
        public int BookingPosition { get; }
        public int Sequencing { get; }
    }
}
