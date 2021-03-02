using System;
using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Campaigns
{
    public class TimeRestriction
    {
        public IEnumerable<string> SalesAreas { get; set; }
        public IEnumerable<string> DowPattern { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string IsIncludeOrExclude { get; set; }
    }
}
