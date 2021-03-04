using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class AdvertiserEntityConfiguration : IEntityTypeConfiguration<Advertiser>
    {
        public void Configure(EntityTypeBuilder<Advertiser> builder)
        {
            builder.ToTable("Advertisers");
            builder.HasKey(k => k.Id);
            builder.Property(p => p.Id).UseMySqlIdentityColumn();

            builder.Property(p => p.Name).HasMaxLength(256);
            builder.Property(p => p.ShortName).HasMaxLength(128);
            builder.Property(p => p.ExternalIdentifier).HasMaxLength(64).IsRequired();
            builder.HasFtsField(Advertiser.SearchFieldName,new string[] { nameof(Advertiser.Name), nameof(Advertiser.ShortName), nameof(Advertiser.ExternalIdentifier) });
            builder.HasIndex(p => p.ExternalIdentifier).IsUnique();
        }
    }
}
