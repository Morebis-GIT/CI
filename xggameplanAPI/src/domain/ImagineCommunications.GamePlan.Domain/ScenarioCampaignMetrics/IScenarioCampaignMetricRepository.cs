using System;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignMetrics.Objects;

namespace ImagineCommunications.GamePlan.Domain.ScenarioCampaignMetrics
{
    public interface IScenarioCampaignMetricRepository
    {
        void AddOrUpdate(ScenarioCampaignMetric scenarioCampaignMetric);
        ScenarioCampaignMetric Get(Guid scenarioId);
        void Delete(Guid scenarioId);
        void SaveChanges();
    }
}
