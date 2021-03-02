using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.KPIComparisonConfigs;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenKPIComparisonConfigRepository : IKPIComparisonConfigRepository
    {
        private readonly IDocumentSession _session;

        public RavenKPIComparisonConfigRepository(IDocumentSession session)
        {
            _session = session;
        }

        public List<KPIComparisonConfig> GetAll()
        {
            lock (_session)
            {
                return _session.GetAll<KPIComparisonConfig>().ToList();
            }
        }
    }
}
