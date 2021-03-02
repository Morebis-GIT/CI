using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Migrations.DbContext
{
    public interface IMigrationDbContext
    {
        DatabaseFacade Database { get; }
    }
}
