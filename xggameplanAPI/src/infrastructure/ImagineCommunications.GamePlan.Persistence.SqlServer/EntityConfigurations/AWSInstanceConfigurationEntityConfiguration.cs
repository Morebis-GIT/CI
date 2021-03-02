using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class AWSInstanceConfigurationEntityConfiguration : IEntityTypeConfiguration<AWSInstanceConfiguration>
    {
        public void Configure(EntityTypeBuilder<AWSInstanceConfiguration> builder)
        {
            builder.ToTable("AWSInstanceConfigurations");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.InstanceType).HasMaxLength(256).IsRequired();
        }
    }
}
