using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenProgrammeEpisodeRepository : IProgrammeEpisodeRepository
    {
        private readonly IDocumentSession _session;

        public RavenProgrammeEpisodeRepository(IDocumentSession session)
        {
            _session = session;
        }

        public IEnumerable<ProgrammeEpisode> GetAll() => _session.GetAll<ProgrammeEpisode>();
    }
}
