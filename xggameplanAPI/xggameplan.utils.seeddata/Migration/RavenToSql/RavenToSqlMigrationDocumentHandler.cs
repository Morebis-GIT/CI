using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Features.Indexed;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Serilog;
using xggameplan.common.Helpers;
using xggameplan.utils.seeddata.SqlServer.Migration.Entities;

namespace xggameplan.utils.seeddata.Migration.RavenToSql
{
    public class RavenToSqlMigrationDocumentHandler<TDomainModel> : MigrationDocumentHandler<TDomainModel>
        where TDomainModel : class
    {
        public RavenToSqlMigrationDocumentHandler(IIndex<MigrationSource, ILifetimeScope> containerIndex, ILogger logger) :
            base(containerIndex, logger)
        {
        }

        public override bool Validate()
        {
            var dbContext = DestinationContainer.Resolve<ISqlServerDbContext>();
            var isMigrated = dbContext.Query<MigrationHistory>()
                .Any(x => x.CollectionName == typeof(TDomainModel).Name);

            if (isMigrated)
            {
                Logger.Warning($"{typeof(TDomainModel).Name} documents have already been migrated.");
                return false;
            }

            var domainModelContext = DestinationContainer.Resolve<IDomainModelContext>();
            var count = domainModelContext.Count<TDomainModel>();
            if (count > 0)
            {
                Logger.Warning($"SqlServer database already contains {typeof(TDomainModel).Name} documents, migration will be skipped.");
                return false;
            }

            return true;
        }

        protected override void SourceModelListPageAction(List<TDomainModel> modelList)
        {
            base.SourceModelListPageAction(modelList);

            var watch = StopwatchHelper.StopwatchAction(() =>
                DestinationContainer.Resolve<ISqlServerDbContext>().SaveChanges());

            Logger.Debug(
                $"{modelList.Count.ToString()} {typeof(TDomainModel).Name} documents have been stored by SqlServer in {watch.Elapsed.ToString()}.");
        }

        public override int Execute()
        {
            var res = base.Execute();

            var dbContext = DestinationContainer.Resolve<ISqlServerDbContext>();
            _ = dbContext.Add(new MigrationHistory
            {
                CollectionName = typeof(TDomainModel).Name,
                Date = DateTime.UtcNow
            });

            dbContext.SaveChanges();

            return res;
        }
    }
}
