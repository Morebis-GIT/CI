using ImagineCommunications.GamePlan.Domain.Generic.DbContext;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces
{
    public interface IRavenTestDbContext : IRavenDbContext, IMasterDbContext, IRavenMasterDbContext, ITenantDbContext, IRavenTenantDbContext
    {
    }
}
