using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.DbSequence;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Services
{
    public class RavenTenantIdentifierSequence : RavenIdentifierSequenceBase, IRavenTenantIdentifierSequence
    {
        public RavenTenantIdentifierSequence(IDocumentSession session) : base(session)
        {
        }
    }
}
