using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Campaigns
{
    public class Timeslice
    {
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public IEnumerable<string> DowPattern { get; set; }
    }
}
