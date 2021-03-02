using System;
using System.Collections.Generic;

namespace xggameplan.core.TestEnvironment
{
    public class TestEnvironmentRunResult
    {
        public Guid RunId { get; set; }

        public Guid ScenarioId { get; set; }

        public int CampaignCount { get; set; }

        public int RecommendationCount { get; set; }

        public IDictionary<string, long> FileNamesAndLengths { get; set; }
    }
}
