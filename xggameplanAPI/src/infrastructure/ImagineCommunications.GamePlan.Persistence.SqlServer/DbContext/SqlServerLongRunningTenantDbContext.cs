using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.DbContext
{
    public class SqlServerLongRunningTenantDbContext : SqlServerTenantDbContext, ISqlServerLongRunningTenantDbContext
    {
        public SqlServerLongRunningTenantDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
        }
    }
}
