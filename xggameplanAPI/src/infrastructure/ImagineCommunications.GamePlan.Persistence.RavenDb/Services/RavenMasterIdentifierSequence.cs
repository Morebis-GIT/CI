using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.DbSequence;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Services
{
    public class RavenMasterIdentifierSequence : RavenIdentifierSequenceBase, IRavenMasterIdentifierSequence
    {
        public RavenMasterIdentifierSequence(IDocumentSession session) : base(session)
        {
        }
    }
}
