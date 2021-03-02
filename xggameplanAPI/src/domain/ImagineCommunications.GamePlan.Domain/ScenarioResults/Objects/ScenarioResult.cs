using System;
using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.ScenarioResults.Objects
{
    /// <summary>
    /// Contains result of AutoBook scenario run
    /// </summary>
    public class ScenarioResult
    {
        public Guid Id { get; set; }
        public DateTime TimeCompleted { get; set; }
        public double? BRSIndicator { get; set; }
        public List<KPI> Metrics { get; set; }
        public List<KPI> LandmarkMetrics { get; set; }
        public List<AnalysisGroupTargetMetric> AnalysisGroupMetrics { get; set; } = new List<AnalysisGroupTargetMetric>();
    }
}
