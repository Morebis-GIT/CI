using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Process.Smooth.Dtos
{
    public class SmoothBatchOutput
    {
        public SmoothBatchOutput()
        {
            OutputByPass = new Dictionary<int, SmoothOutputForPass>();
            SpotsByFailureMessage = new Dictionary<int, int>();
            UnusedSpotIds = new HashSet<Guid>();
            UsedSpotIds = new HashSet<Guid>();
        }

        public int BookedSpotsUnplacedDueToRestrictions { get; set; }
        public int Breaks { get; set; }
        public int Failures { get; set; }
        public int Recommendations { get; set; }
        public int SpotsNotSetDueToExternalCampaignRef { get; set; }
        public int SpotsSetAfterMovingOtherSpots { get; set; }
        public int SpotsSet { get; set; }

        public IDictionary<int, SmoothOutputForPass> OutputByPass { get; }
        public IDictionary<int, int> SpotsByFailureMessage { get; }
        public HashSet<Guid> UnusedSpotIds { get; }
        public HashSet<Guid> UsedSpotIds { get; }
    }
}
