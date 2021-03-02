using System.Collections.Generic;
using NodaTime;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Campaigns
{
    public class Multipart
    {
        public int MultipartNumber { get; set; }
        public IEnumerable<Duration> Lengths { get; set; }
        public decimal DesiredPercentageSplit { get; set; }
        public decimal CurrentPercentageSplit { get; set; }
    }
}
