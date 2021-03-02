using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.DbContext;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.DbContext
{
    public class SqlServerMasterDbContext : SqlServerDbContext, ISqlServerMasterDbContext
    {
        public SqlServerMasterDbContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AccessTokenEntityConfiguration());
            modelBuilder.ApplyConfiguration(new PreviewFileEntityConfiguration());
            modelBuilder.ApplyConfiguration(new TenantProductFeatureEntityConfiguration());
            modelBuilder.ApplyConfiguration(new TenantProductFeatureReferenceEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ProductSettingsEntityConfiguration());
            modelBuilder.ApplyConfiguration(new ProductSettingFeatureEntityConfiguration());
            modelBuilder.ApplyConfiguration(new TaskInstanceEntityConfiguration());
            modelBuilder.ApplyConfiguration(new TaskInstanceParameterEntityConfiguration());
            modelBuilder.ApplyConfiguration(new TenantEntityConfiguration());
            modelBuilder.ApplyConfiguration(new UpdateDetailsEntityConfiguration());
            modelBuilder.ApplyConfiguration(new UserEntityConfiguration());
            modelBuilder.ApplyConfiguration(new UserSettingEntityConfiguration());
        }
    }
}
