using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Runs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Runs
{
    public class LandmarkRunQueueEntityConfiguration : IEntityTypeConfiguration<LandmarkRunQueue>
    {
        public void Configure(EntityTypeBuilder<LandmarkRunQueue> builder)
        {
            builder.ToTable("LandmarkRunQueues");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseSqlServerIdentityColumn();
            builder.Property(x => x.Name).IsRequired().HasMaxLength(64);
            builder.Property(x => x.DisplayName).HasMaxLength(128);
        }
    }
}
