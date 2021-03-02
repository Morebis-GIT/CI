using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos
{
    /// <summary>
    /// Filter for spots
    /// </summary>
    public class SpotFilter
    {
        public int? MinPreemptLevel { get; set; }
        public int? MaxPreemptLevel { get; set; }
        public bool? Sponsored { get; set; }
        public bool? Preemptable { get; set; }
        public bool? HasBreakRequest { get; set; }
        public List<string> BreakRequests { get; set; }
        public bool? HasPositionInBreakRequest { get; set; }
        public List<string> PositionInBreakRequestsToExclude { get; set; }
        public bool? HasMultipartSpots { get; set; }
        public List<string> MultipartSpots { get; set; }
        public bool? HasProductClashCode { get; set; }
        public List<string> ExternalCampaignRefsToExclude { get; set; }
        public List<string> ProductClashCodesToExclude { get; set; }
        public bool? HasSpotEndTime { get; set; }

        public TimeSpan? MinSpotLength { get; set; }
        public TimeSpan? MaxSpotLength { get; set; }

        public ICollection<Guid> SpotIdsToExclude { get; set; }

        public List<string> ProductAdvertiserIdentifiersToExclude { get; set; }
    }
}
