using NodaTime;

namespace xggameplan.Model
{
    public class DayPartLengthModel
    {
        public Duration Length { get; set; }
        public int MultipartNumber { get; set; }
        public decimal DesiredPercentageSplit { get; set; }
        public decimal CurrentPercentageSplit { get; set; }
    }
}
