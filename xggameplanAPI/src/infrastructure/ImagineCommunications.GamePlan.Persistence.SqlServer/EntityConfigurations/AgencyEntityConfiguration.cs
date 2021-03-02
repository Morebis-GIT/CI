using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class AgencyEntityConfiguration : IEntityTypeConfiguration<Agency>
    {
        public void Configure(EntityTypeBuilder<Agency> builder)
        {
            builder.ToTable("Agencies");
            builder.HasKey(k => k.Id);
            builder.Property(p => p.Id).UseMySqlIdentityColumn();

            builder.Property(p => p.Name).HasMaxLength(256);
            builder.Property(p => p.ShortName).HasMaxLength(128);
            builder.Property(p => p.ExternalIdentifier).HasMaxLength(64).IsRequired();
            builder.Property<string>(Agency.SearchFieldName).HasMaxLength(TextColumnLenght.SearchField);
            builder.HasIndex(p => p.ExternalIdentifier).IsUnique();

            builder.HasFtsField(Agency.SearchFieldName, Agency.SearchFieldSources);
        }
    }
}
