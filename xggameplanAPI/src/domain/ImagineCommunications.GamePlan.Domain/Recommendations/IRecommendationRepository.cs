using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;

namespace ImagineCommunications.GamePlan.Domain.Recommendations
{
    public interface IRecommendationRepository
    {
        void Insert(IEnumerable<Recommendation> recommendations, bool setIdentity = true);

        IEnumerable<Recommendation> GetByScenarioId(Guid scenarioId);

        IEnumerable<Recommendation> GetByScenarioIdAndProcessors(Guid scenarioId, IEnumerable<string> processors);

        IEnumerable<RecommendationsByScenarioReduceResult> GetMetrics(Guid scenarioId, string campaignId);

        IEnumerable<RecommendationsByScenarioReduceResult> GetCampaigns(Guid scenarioId);

        void RemoveByScenarioId(Guid scenarioId);

        void RemoveByScenarioIdAndProcessors(Guid scenarioId, IEnumerable<string> processors);

        IEnumerable<RecommendationSimple> GetRecommendationSimplesByScenarioIdAndProcessors(Guid scenarioId, IEnumerable<string> processors);

        IEnumerable<RecommendationSimple> GetRecommendationSimplesByScenarioIdsAndProcessors(List<Guid> scenarioIds, IEnumerable<string> processors);

        void SaveChanges();
    }
}
