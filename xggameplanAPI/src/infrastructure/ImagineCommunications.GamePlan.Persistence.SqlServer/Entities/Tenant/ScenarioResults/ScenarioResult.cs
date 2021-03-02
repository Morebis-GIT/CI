using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ScenarioResults
{
    public class ScenarioResult : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public Guid ScenarioId { get; set; }
        public DateTime TimeCompleted { get; set; }
        public double? BRSIndicator { get; set; }
        public virtual List<LandmarkScenarioResultMetric> LandmarkMetrics { get; set; } = new List<LandmarkScenarioResultMetric>();
        public virtual List<GameplanScenarioResultMetric> Metrics { get; set; } = new List<GameplanScenarioResultMetric>();
        public ICollection<AnalysisGroupTargetMetric> AnalysisGroupMetrics { get; set; } = new HashSet<AnalysisGroupTargetMetric>();
    }
}
