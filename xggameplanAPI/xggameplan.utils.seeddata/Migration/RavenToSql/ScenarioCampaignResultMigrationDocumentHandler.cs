using Autofac;
using Autofac.Features.Indexed;
using ImagineCommunications.GamePlan.Domain.ScenarioCampaignResults.Objects;
using Serilog;

namespace xggameplan.utils.seeddata.Migration.RavenToSql
{
    public class ScenarioCampaignResultMigrationDocumentHandler : RavenToSqlMigrationDocumentHandler<ScenarioCampaignResult>
    {
        public ScenarioCampaignResultMigrationDocumentHandler(IIndex<MigrationSource, ILifetimeScope> containerIndex, ILogger logger)
            : base(containerIndex, logger)
        {
        }

        public override int PageSize => 1;
    }
}
