using ImagineCommunications.GamePlan.Domain.Generic.DbContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DbContext;
using xggameplan.common.ActionProcessing;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces
{
    public interface ISqlServerDbContext : IDbContext<SqlServerSpecificDbAdapter>
    {
        ISqlServerBulkInsertEngine BulkInsertEngine { get; }

        IActionCollection PostActions { get; }
    }
}
