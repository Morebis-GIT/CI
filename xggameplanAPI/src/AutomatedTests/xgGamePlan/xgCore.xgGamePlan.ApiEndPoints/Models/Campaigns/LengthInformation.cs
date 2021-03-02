using NodaTime;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Campaigns
{
    public class LengthInformation 
    {
        public int MultipartNumber { get; set; }
        public Duration Length { get; set; }
        public decimal DesiredPercentageSplit { get; set; }
        public decimal CurrentPercentageSplit { get; set; }
    }
}
