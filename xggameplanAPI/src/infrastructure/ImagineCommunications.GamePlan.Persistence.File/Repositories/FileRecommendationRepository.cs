using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Recommendations;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using xgcore.auditevents.Repository.File;

namespace ImagineCommunications.GamePlan.Persistence.File.Repositories
{
    public class FileRecommendationRepository : FileRepositoryBase, IRecommendationRepository
    {
        public FileRecommendationRepository(string folder) : base(folder, "recommendation")
        {

        }

        public void Dispose()
        {

        }

        public void Insert(IEnumerable<Recommendation> recommendations, bool setIdentity = true)
        {
            InsertItems(_folder, _type, recommendations.ToList(), recommendations.Select(i => i.Id.ToString()).ToList());
        }

        public IEnumerable<Recommendation> GetByScenarioId(Guid scenarioId)
        {
            return GetAllByType<Recommendation>(_folder, _type, r => r.ScenarioId == scenarioId);       // TODO: Optimize
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
            return GetAllByType<Recommendation>(_folder, _type, recommendation => recommendation.ScenarioId == scenarioId && processors.Contains(recommendation.Processor));
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
            DeleteAllItems<Recommendation>(_folder, _type, r => r.ScenarioId == scenarioId);
        }

        public void RemoveByScenarioIdAndProcessors(Guid scenarioId, IEnumerable<string> processors)
        {
            DeleteAllItems<Recommendation>(_folder, _type, r => r.ScenarioId == scenarioId && processors.Contains(r.Processor));
        }

        public void SaveChanges()
        {

        }
    }
}
