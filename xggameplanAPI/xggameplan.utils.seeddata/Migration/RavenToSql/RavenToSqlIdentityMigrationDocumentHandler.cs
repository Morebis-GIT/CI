using System.Collections.Generic;
using Autofac;
using Autofac.Features.Indexed;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Extensions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace xggameplan.utils.seeddata.Migration.RavenToSql
{
    public class RavenToSqlIdentityMigrationDocumentHandler<TDomainModel, TIdentityEntity> :
        RavenToSqlMigrationDocumentHandler<TDomainModel>
        where TDomainModel : class
        where TIdentityEntity : class
    {
        public RavenToSqlIdentityMigrationDocumentHandler(
            IIndex<MigrationSource, ILifetimeScope> containerIndex, ILogger logger) : base(containerIndex, logger)
        {
        }

        protected override void SourceModelListPageAction(List<TDomainModel> modelList)
        {
            var dbContext = DestinationContainer.Resolve<ISqlServerDbContext>();
            using var connection = dbContext.Specific.Database.GetDbConnection();

            dbContext.Specific.Database.OpenConnection();
            var tableName = dbContext.Specific.Model.GetFullTableName<TIdentityEntity>();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = $"SET IDENTITY_INSERT {tableName} ON;";
                _ = command.ExecuteNonQuery();
            }

            try
            {
                base.SourceModelListPageAction(modelList);
            }
            finally
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"SET IDENTITY_INSERT {tableName} OFF;";
                    _ = command.ExecuteNonQuery();
                }
            }
        }

        public override bool ProcessDestinationAsABatch { get; set; } = false;
    }
}
