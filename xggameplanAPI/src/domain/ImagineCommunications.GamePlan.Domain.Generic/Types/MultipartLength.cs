using NodaTime;

namespace ImagineCommunications.GamePlan.Domain.Generic.Types
{
    public class MultipartLength
    {
        public Duration Length { get; set; }
        public int BookingPosition { get; set; }
        public int Sequencing { get; set; }
    }
}
