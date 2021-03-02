using Autofac;
using Autofac.Features.Indexed;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.PredictionSchedules;
using Serilog;

namespace xggameplan.utils.seeddata.Migration.RavenToSql
{
    public class RatingsPredictionScheduleMigrationDocumentHandler
        : RavenToSqlIdentityMigrationDocumentHandler<RatingsPredictionSchedule, PredictionSchedule>
    {
        public RatingsPredictionScheduleMigrationDocumentHandler(IIndex<MigrationSource, ILifetimeScope> containerIndex,
            ILogger logger) : base(containerIndex, logger)
        {
        }

        public override int PageSize => 10;
    }
}
