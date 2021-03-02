using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;

namespace xggameplan.OutputFiles.Processing
{
    /// <summary>
    /// Output for processing of SpotsReqm output file.
    /// </summary>
    public class SpotsReqmOutput
    {
        //public int CampaignPerformance { get; set; }

        //public int SpotPerformance { get; set; }

        //public double NewEfficiency { get; set; }

        public Guid ScenarioId { get; set; }
        public int CountTotalSpots { get; set; }
        public int CountCancelledSpots { get; set; }
        public List<Recommendation> Recommendations = new List<Recommendation>();
    }
}
