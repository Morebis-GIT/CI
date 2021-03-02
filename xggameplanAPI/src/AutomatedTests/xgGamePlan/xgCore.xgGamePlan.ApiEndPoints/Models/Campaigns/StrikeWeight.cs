using System;
using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Campaigns
{
    public class StrikeWeight
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal DesiredPercentageSplit { get; set; }
        public decimal CurrentPercentageSplit { get; set; }
        public IEnumerable<LengthInformation > Lengths { get; set; }
        public IEnumerable<DayPart> DayParts { get; set; }
    }
}
