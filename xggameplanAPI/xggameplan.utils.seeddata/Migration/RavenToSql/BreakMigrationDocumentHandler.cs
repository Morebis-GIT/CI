using Autofac;
using Autofac.Features.Indexed;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using Serilog;
using BreakEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Breaks.Break;

namespace xggameplan.utils.seeddata.Migration.RavenToSql
{
    public class BreakMigrationDocumentHandler
        : RavenToSqlIdentityMigrationDocumentHandler<Break, BreakEntity>
    {
        public BreakMigrationDocumentHandler(
            IIndex<MigrationSource, ILifetimeScope> containerIndex,
            ILogger logger) : base(containerIndex, logger)
        {
        }

        public override int PageSize => 512;
    }
}
