using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ScenarioResults
{
    public class AnalysisGroupTargetMetric : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int ScenarioResultId { get; set; }
        public Guid AnalysisGroupTargetId { get; set; }
        public double Value { get; set; }
    }
}
