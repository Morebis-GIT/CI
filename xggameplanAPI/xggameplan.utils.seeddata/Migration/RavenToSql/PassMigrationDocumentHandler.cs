using System;
using Autofac;
using Autofac.Features.Indexed;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Helpers;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Serilog;
using xggameplan.Model;
using PassEntity = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes.Pass;

namespace xggameplan.utils.seeddata.Migration.RavenToSql
{
    public class PassMigrationDocumentHandler : RavenToSqlMigrationDocumentHandler<Pass>
    {
        private readonly SequenceRebuilder<PassEntity, PassIdIdentity> _sequenceRebuilder;

        public PassMigrationDocumentHandler(
            SequenceRebuilder<PassEntity, PassIdIdentity> sequenceRebuilder,
            IIndex<MigrationSource, ILifetimeScope> containerIndex,
            ILogger logger) : base(containerIndex, logger)
        {
            _sequenceRebuilder = sequenceRebuilder ?? throw new ArgumentNullException(nameof(sequenceRebuilder));
        }

        public override int Execute()
        {
            var res = base.Execute();

            var dbContext = DestinationContainer.Resolve<ISqlServerDbContext>();
            _sequenceRebuilder.Execute(dbContext, x => x.Id);

            return res;
        }

        public override int PageSize => 100;
    }
}
