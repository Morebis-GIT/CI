using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces
{
    public interface ISqlServerTestDbContext : ISqlServerMasterDbContext, ISqlServerTenantDbContext, ISqlServerLongRunningTenantDbContext
    {
    }
}
