using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.DbContext;
using Raven.Client;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.RavenDB
{
    public class RavenTestDbContext : RavenDbContext, IRavenTestDbContext
    {
        public RavenTestDbContext(IDocumentSession documentSession, IAsyncDocumentSession asyncDocumentSession) : base(documentSession, asyncDocumentSession)
        {
        }
    }
}
