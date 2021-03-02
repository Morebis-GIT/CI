using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Clashes
{
    public class UpdateClashModel
    {
        public string ParentExternalidentifier { get; set; }
        public string Description { get; set; }
        public int DefaultPeakExposureCount { get; set; }
        public int DefaultOffPeakExposureCount { get; set; }

        public IEnumerable<ClashDifference> Differences { get; set; }
    }
}
