using System;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults.Objects;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenScenarioCampaignResultsRepository : IScenarioCampaignResultRepository
    {
        private readonly IDocumentSession _session;

        public RavenScenarioCampaignResultsRepository(IDocumentSession session)
            => _session = session;

        public void AddOrUpdate(ScenarioCampaignResult scenarioCampaignResults)
        {
            lock (_session)
            {
                _session.Store(scenarioCampaignResults);
            }
        }

        public void Delete(Guid scenarioId)
        {
            lock (_session)
            {
                var scenarioCampaignResults = _session.Load<ScenarioCampaignResult>(scenarioId);
                if (scenarioCampaignResults != null)
                {
                    _session.Delete(scenarioCampaignResults);
                }
            }
        }

        public ScenarioCampaignResult Get(Guid scenarioId)
            => _session.Load<ScenarioCampaignResult>(scenarioId);

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }
    }
}
