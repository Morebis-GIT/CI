using ImagineCommunications.Gameplan.Integration.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.DbContext
{
    public class IntelligenceMigrationDbContext : IntelligenceDbContext, IMigrationDbContext
    {
        public IntelligenceMigrationDbContext(DbContextOptions<IntelligenceDbContext> dbContextOptions)
            : base(dbContextOptions)
        {
        }
    }
}
