using Autofac;
using Autofac.Features.Indexed;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using Serilog;
using ScheduleEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Schedules.Schedule;

namespace xggameplan.utils.seeddata.Migration.RavenToSql
{
    public class ScheduleMigrationDocumentHandler : RavenToSqlIdentityMigrationDocumentHandler<Schedule, ScheduleEntity>
    {
        public ScheduleMigrationDocumentHandler(IIndex<MigrationSource, ILifetimeScope> containerIndex, ILogger logger)
            : base(containerIndex, logger)
        {
        }

        public override int PageSize => 10;
    }
}
