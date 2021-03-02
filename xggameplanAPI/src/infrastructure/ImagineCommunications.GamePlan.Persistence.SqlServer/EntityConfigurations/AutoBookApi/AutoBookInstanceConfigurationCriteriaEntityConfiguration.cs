using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.AutoBookApi
{
    public class AutoBookInstanceConfigurationCriteriaEntityConfiguration : IEntityTypeConfiguration<AutoBookInstanceConfigurationCriteria>
    {
        public void Configure(EntityTypeBuilder<AutoBookInstanceConfigurationCriteria> builder)
        {
            builder.ToTable("AutoBookInstanceConfigurationCriterias");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseSqlServerIdentityColumn();

            builder.HasIndex(x => x.AutoBookInstanceConfigurationId);
        }
    }
}
