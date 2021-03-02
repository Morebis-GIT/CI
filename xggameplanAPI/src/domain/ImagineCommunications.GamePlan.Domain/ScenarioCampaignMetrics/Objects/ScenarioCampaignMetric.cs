using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.ScenarioCampaignMetrics.Objects
{
    public class ScenarioCampaignMetric
    {
        /// <summary>
        /// ScenarioId
        /// </summary>
        public Guid Id { get; set; }
        public List<ScenarioCampaignMetricItem> Metrics { get; set; }
    }
}
