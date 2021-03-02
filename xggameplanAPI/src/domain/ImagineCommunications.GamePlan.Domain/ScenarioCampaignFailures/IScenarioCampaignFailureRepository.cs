using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Queries;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures.Objects;

namespace ImagineCommunications.GamePlan.Domain.ScenarioCampaignFailures
{
    public interface IScenarioCampaignFailureRepository
    {
        void Add(ScenarioCampaignFailure scenarioCampaignFailure);
        void AddRange(IEnumerable<ScenarioCampaignFailure> scenarioCampaignFailures, bool setIdentity = true);
        void Delete(int Id);
        void RemoveByScenarioId(Guid Id);
        ScenarioCampaignFailure Get(int Id);
        IEnumerable<ScenarioCampaignFailure> GetAll();
        IEnumerable<ScenarioCampaignFailure> FindByScenarioId(Guid scenarioId);
        PagedQueryResult<ScenarioCampaignFailure> Search(ScenarioCampaignFailureSearchQueryModel searchQuery);
        void SaveChanges();
    }
}
