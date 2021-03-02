using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.BRS;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenKPIPriorityRepository : IKPIPriorityRepository
    {
        private readonly IDocumentSession _session;

        public RavenKPIPriorityRepository(IDocumentSession session)
        {
            _session = session;
        }

        public IEnumerable<KPIPriority> GetAll() => _session.GetAll<KPIPriority>();
    }
}
