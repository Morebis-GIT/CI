using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Smooth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Smooth
{
    public class SmoothFailureMessageDescriptionEntityTypeConfiguration : IEntityTypeConfiguration<SmoothFailureMessageDescription>
    {
        public void Configure(EntityTypeBuilder<SmoothFailureMessageDescription> builder)
        {
            builder.ToTable("SmoothFailureMessageDescriptions");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseSqlServerIdentityColumn();

            builder.Property(x => x.Description).IsRequired();
            builder.Property(x => x.LanguageAbbreviation).HasMaxLength(3).IsRequired();

            builder.HasIndex(x => x.SmoothFailureMessageId);
        }
    }
}
