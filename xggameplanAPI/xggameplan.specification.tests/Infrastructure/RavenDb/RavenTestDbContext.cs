using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.DbContext;
using Raven.Client;
using xggameplan.specification.tests.Interfaces;

namespace xggameplan.specification.tests.Infrastructure.RavenDb
{
    public class RavenTestDbContext : RavenDbContext, IRavenTestDbContext
    {
        public RavenTestDbContext(IDocumentSession documentSession, IAsyncDocumentSession asyncDocumentSession) : base(documentSession, asyncDocumentSession)
        {
        }
    }
}
