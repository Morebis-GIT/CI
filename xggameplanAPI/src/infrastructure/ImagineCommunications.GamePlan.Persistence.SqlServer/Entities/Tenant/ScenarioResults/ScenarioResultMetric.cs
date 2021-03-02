using ImagineCommunications.GamePlan.Domain;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ScenarioResults
{
    public abstract class ScenarioResultMetric : IIdentityPrimaryKey
    {
        public int Id { get; set; }
        public int ScenarioResultId { get; set; }
        public string Name { get; set; }
        public string DisplayFormat { get; set; }
        public double Value { get; set; }
        public KPISource ResultSource { get; set; }
    }
}
