using ImagineCommunications.GamePlan.Domain.Generic.DbContext;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces;

namespace xggameplan.specification.tests.Interfaces
{
    public interface IRavenTestDbContext : IRavenDbContext, IMasterDbContext, IRavenMasterDbContext, ITenantDbContext, IRavenTenantDbContext
    {
    }
}
