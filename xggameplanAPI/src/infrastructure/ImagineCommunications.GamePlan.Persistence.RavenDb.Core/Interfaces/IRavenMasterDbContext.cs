using ImagineCommunications.GamePlan.Domain.Generic.DbContext;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.DbContext;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces
{
    public interface IRavenMasterDbContext : IRavenDbContext, IMasterDbContext<RavenSpecificDbAdapter>
    {
    }
}
