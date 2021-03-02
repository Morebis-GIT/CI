using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Recommendations;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;

namespace ImagineCommunications.GamePlan.Persistence.Memory.Repositories
{
    public class MemoryRecommendationRepository :
        MemoryRepositoryBase<Recommendation>,
        IRecommendationRepository
    {
        public MemoryRecommendationRepository()
        {
        }

        public void Dispose()
        {
        }

        public void Insert(IEnumerable<Recommendation> recommendations, bool setIdentity = true)
        {
            InsertItems(recommendations.ToList(), recommendations.Select(i => i.Id.ToString()).ToList<string>());
        }

        public IEnumerable<Recommendation> GetByScenarioId(Guid scenarioId)
        {
            return GetAllItems(r => r.ScenarioId == scenarioId);
        }

        public IEnumerable<RecommendationSimple> GetRecommendationSimplesByScenarioIdAndProcessors(Guid scenarioId, IEnumerable<string> processors)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RecommendationSimple> GetRecommendationSimplesByScenarioIdsAndProcessors(List<Guid> scenarioIds, IEnumerable<string> processors)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Recommendation> GetByScenarioIdAndProcessors(Guid scenarioId, IEnumerable<string> processors)
        {
            return GetAllItems(recommendation => recommendation.ScenarioId == scenarioId && processors.Contains(recommendation.Processor));
        }

        public IEnumerable<RecommendationsByScenarioReduceResult> GetCampaigns(Guid scenarioId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<RecommendationsByScenarioReduceResult> GetMetrics(Guid scenarioId, string campaignId)
        {
            throw new NotImplementedException();
        }

        public void RemoveByScenarioId(Guid scenarioId)
        {
            DeleteAllItems(r => r.ScenarioId == scenarioId);
        }

        public void RemoveByScenarioIdAndProcessors(Guid scenarioId, IEnumerable<string> processors)
        {
            DeleteAllItems(r => r.ScenarioId == scenarioId && processors.Contains(r.Processor));
        }

        public void SaveChanges()
        {
        }
    }
}
