using System;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults.Objects;

namespace ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults
{
    public interface IScenarioCampaignResultRepository
    {
        void AddOrUpdate(ScenarioCampaignResult scenarioCampaignResults);
        ScenarioCampaignResult Get(Guid scenarioId);
        void Delete(Guid scenarioId);
        void SaveChanges();
    }
}
