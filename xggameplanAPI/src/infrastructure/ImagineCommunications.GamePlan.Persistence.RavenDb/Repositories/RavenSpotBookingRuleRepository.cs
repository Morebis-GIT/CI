using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.SpotBookingRules;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenSpotBookingRuleRepository : ISpotBookingRuleRepository
    {
        private readonly IDocumentSession _session;

        public RavenSpotBookingRuleRepository(IDocumentSession session)
        {
            _session = session;
        }

        public IEnumerable<SpotBookingRule> GetAll() => _session.GetAll<SpotBookingRule>();

        public SpotBookingRule Get(int id) => _session.Load<SpotBookingRule>(id);

        public void AddRange(IEnumerable<SpotBookingRule> spotBookingRules)
        {
            lock (_session)
            {
                var options = new BulkInsertOptions { OverwriteExisting = true };
                using (var bulkInsert = _session.Advanced.DocumentStore.BulkInsert(null, options))
                {
                    spotBookingRules.ForEach(item => bulkInsert.Store(item));
                }
            }
        }

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
            }
        }

        public void Truncate() => _session.TryDelete("Raven/DocumentsByEntityName", "SpotBookingRules");
    }
}
