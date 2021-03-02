using NodaTime;

namespace xggameplan.Model
{
    public class LengthModel
    {
        public int MultipartNumber { get; set; }
        public Duration Length { get; set; }
        public decimal DesiredPercentageSplit { get; set; }
        public decimal CurrentPercentageSplit { get; set; }
    }
}
