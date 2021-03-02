using ImagineCommunications.GamePlan.Domain.Generic.DbContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DbContext;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces
{
    public interface ISqlServerTenantDbContext : ISqlServerDbContext, ITenantDbContext<SqlServerSpecificDbAdapter>
    {
    }
}
