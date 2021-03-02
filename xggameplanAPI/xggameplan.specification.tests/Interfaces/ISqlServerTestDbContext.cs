using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace xggameplan.specification.tests.Interfaces
{
    public interface ISqlServerTestDbContext : ISqlServerMasterDbContext, ISqlServerTenantDbContext, ISqlServerLongRunningTenantDbContext
    {
    }
}
