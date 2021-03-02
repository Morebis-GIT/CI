using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.DbContext;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.DbContext
{
    public class TenantRavenDbContext : RavenDbContext, IRavenTenantDbContext
    {
        public TenantRavenDbContext(IDocumentSession documentSession, IAsyncDocumentSession asyncDocumentSession) :
            base(documentSession, asyncDocumentSession)
        {
        }
    }
}
