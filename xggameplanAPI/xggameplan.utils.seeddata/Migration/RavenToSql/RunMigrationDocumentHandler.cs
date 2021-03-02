using System;
using Autofac;
using Autofac.Features.Indexed;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Helpers;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Serilog;
using xggameplan.Model;
using RunEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs.Run;

namespace xggameplan.utils.seeddata.Migration.RavenToSql
{
    public class RunMigrationDocumentHandler : RavenToSqlMigrationDocumentHandler<Run>
    {
        private readonly SequenceRebuilder<RunEntity, RunNoIdentity> _sequenceRebuilder;

        public RunMigrationDocumentHandler(
            SequenceRebuilder<RunEntity, RunNoIdentity> sequenceRebuilder,
            IIndex<MigrationSource, ILifetimeScope> containerIndex,
            ILogger logger) : base(containerIndex, logger)
        {
            _sequenceRebuilder = sequenceRebuilder ?? throw new ArgumentNullException(nameof(sequenceRebuilder));
        }

        public override int Execute()
        {
            var res = base.Execute();

            var dbContext = DestinationContainer.Resolve<ISqlServerDbContext>();
            _sequenceRebuilder.Execute(dbContext, x => x.CustomId);

            return res;
        }
    }
}
