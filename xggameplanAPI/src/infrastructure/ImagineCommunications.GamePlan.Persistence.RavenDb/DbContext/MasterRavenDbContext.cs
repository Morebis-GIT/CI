using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.DbContext;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.DbContext
{
    public class MasterRavenDbContext : RavenDbContext, IRavenMasterDbContext
    {
        public MasterRavenDbContext(IDocumentSession documentSession, IAsyncDocumentSession asyncDocumentSession) :
            base(documentSession, asyncDocumentSession)
        {
        }
    }
}
