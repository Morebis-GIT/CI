using System.Collections.Generic;
using Raven.Client;
using ImagineCommunications.GamePlan.Domain.LandmarkRunQueues;
using ImagineCommunications.GamePlan.Domain.LandmarkRunQueues.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenLandmarkRunQueueRepository : ILandmarkRunQueueRepository
    {
        private readonly IDocumentSession _session;

        public RavenLandmarkRunQueueRepository(IDocumentSession session)
        {
            _session = session;
        }

        public IEnumerable<LandmarkRunQueue> GetAll() => _session.GetAll<LandmarkRunQueue>();
    }
}
