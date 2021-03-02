using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Campaigns
{
    public class DayPart
    {
        public decimal DesiredPercentageSplit { get; set; }
        public decimal CurrentPercentageSplit { get; set; }
        public IEnumerable<Timeslice> Timeslices { get; set; }
        public IEnumerable<DayPartLength> Lengths { get; set; }
    }
}
