using Autofac;
using Autofac.Features.Indexed;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using Serilog;
using MasterEntities = ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master;

namespace xggameplan.utils.seeddata.Migration.RavenToSql
{
    public class TenantMigrationDocumentHandler :
        PreviewableEntityMigrationDocumentHandler<Tenant, MasterEntities.Tenant, ITenantsRepository>
    {
        public TenantMigrationDocumentHandler(IIndex<MigrationSource, ILifetimeScope> containerIndex, ILogger logger)
            : base(containerIndex, logger)
        {
        }

        protected override void AdjustModel(Tenant model)
        {
            var dbProviderConfig = DatabaseProviderConfiguration.CreateFromConfiguration(model.TenantDb.Provider, model.TenantDb.ConfigurationJson);
            model.Change(model.Name, model.DefaultTheme, dbProviderConfig);
        }
    }
}
