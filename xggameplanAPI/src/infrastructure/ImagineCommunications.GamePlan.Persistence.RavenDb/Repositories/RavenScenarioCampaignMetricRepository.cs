using System;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignMetrics;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignMetrics.Objects;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenScenarioCampaignMetricRepository : IScenarioCampaignMetricRepository
    {
        private readonly IDocumentSession _session;

        public RavenScenarioCampaignMetricRepository(IDocumentSession session)
            => _session = session;

        public void AddOrUpdate(ScenarioCampaignMetric scenarioCampaignMetric)
        {
            lock (_session)
            {
                _session.Store(scenarioCampaignMetric);
            }
        }

        public void Delete(Guid scenarioId)
        {
            lock (_session)
            {
                var scenarioCampaignMetric = _session.Load<ScenarioCampaignMetric>(scenarioId);
                if (scenarioCampaignMetric != null)
                {
                    _session.Delete(scenarioCampaignMetric);
                }
            }
        }

        public ScenarioCampaignMetric Get(Guid scenarioId)
            => _session.Load<ScenarioCampaignMetric>(scenarioId);

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }
    }
}
