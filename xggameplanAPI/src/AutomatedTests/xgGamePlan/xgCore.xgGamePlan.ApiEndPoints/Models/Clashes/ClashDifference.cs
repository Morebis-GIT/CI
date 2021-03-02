using System;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Clashes
{
    public class ClashDifference
    {
        public string SalesArea { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ClashDifferenceTimeAndDow TimeAndDow { get; set; }
        public int? PeakExposureCount { get; set; }
        public int? OffPeakExposureCount { get; set; }
    }
}
