using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth.Configuration.SmoothPassEntities;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Smooth.Configuration
{
    public class SmoothPassEntityTypeConfiguration : IEntityTypeConfiguration<SmoothPass>
    {
        public void Configure(EntityTypeBuilder<SmoothPass> builder)
        {
            builder.ToTable("SmoothConfigurationSmoothPasses");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).UseSqlServerIdentityColumn();

            builder.HasDiscriminator<SmoothPassType>("SmoothPassType")
                .HasValue<SmoothPassDefault>(SmoothPassType.Default)
                .HasValue<SmoothPassBooked>(SmoothPassType.Booked)
                .HasValue<SmoothPassUnplaced>(SmoothPassType.Unplaced);
        }
    }

    public class SmoothPassDefaultEntityTypeConfiguration : IEntityTypeConfiguration<SmoothPassDefault>
    {
        public void Configure(EntityTypeBuilder<SmoothPassDefault> builder)
        {
            builder.HasBaseType<SmoothPass>();
            builder.Property(x => x.BreakRequests).AsDelimitedString();
        }
    }

    public class SmoothPassBookedEntityTypeConfiguration : IEntityTypeConfiguration<SmoothPassBooked>
    {
        public void Configure(EntityTypeBuilder<SmoothPassBooked> builder)
        {
            builder.HasBaseType<SmoothPass>();
        }
    }

    public class SmoothPassUnplacedEntityTypeConfiguration : IEntityTypeConfiguration<SmoothPassUnplaced>
    {
        public void Configure(EntityTypeBuilder<SmoothPassUnplaced> builder)
        {
            builder.HasBaseType<SmoothPass>();
        }
    }
}
