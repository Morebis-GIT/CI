using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class ChannelEntityConfiguration : IEntityTypeConfiguration<Channel>
    {
        public void Configure(EntityTypeBuilder<Channel> builder)
        {
            builder.ToTable("Channels");
            builder.HasKey(k => k.Id);
            builder.Property(p => p.Id).UseSqlServerIdentityColumn();

            builder.Property(p => p.Name).HasMaxLength(256);
            builder.Property(p => p.ShortName).HasMaxLength(128);

            builder.HasIndex(p => p.Name).IsUnique();
            builder.HasIndex(p => p.ShortName).IsUnique();
        }
    }
}
