using System;
using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Runs
{
    public class TestEnvironmentRunResult
    {
        public Guid RunId { get; set; }

        public Guid ScenarioId { get; set; }

        public int CampaignCount { get; set; }

        public int RecommendationCount { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
        public Dictionary<string, long> FileNamesAndLengths { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
    }
}
