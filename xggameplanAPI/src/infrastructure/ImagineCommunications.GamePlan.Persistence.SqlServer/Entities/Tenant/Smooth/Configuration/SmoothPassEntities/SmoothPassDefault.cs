using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.SmoothPassEntities
{
    public class SmoothPassDefault : SmoothPass
    {
        public bool? Sponsored { get; set; }

        public bool? HasMultipartSpots { get; set; }

        public bool? Preemptable { get; set; }

        public ICollection<string> BreakRequests { get; set; }

        public bool? HasProductClashCode { get; set; }

        public bool CanSplitMultipartSpots { get; set; }

        public bool? HasSpotEndTime { get; set; }
    }
}
