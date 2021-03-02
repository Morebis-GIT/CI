using ImagineCommunications.GamePlan.Domain.Generic.DbContext;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.DbContext;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces
{
    public interface IRavenDbContext : IDbContext<RavenSpecificDbAdapter>
    {
        IRavenDbBulkInsertEngine BulkInsertEngine { get; }
    }
}
