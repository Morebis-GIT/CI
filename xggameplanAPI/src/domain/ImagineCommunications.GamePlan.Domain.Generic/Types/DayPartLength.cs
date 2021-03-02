using NodaTime;

namespace ImagineCommunications.GamePlan.Domain.Generic.Types
{
    public class DayPartLength
    {
        public Duration Length { get; set; }
        public int MultipartNumber { get; set; }
        public decimal DesiredPercentageSplit { get; set; }
        public decimal CurrentPercentageSplit { get; set; }
    }
}
