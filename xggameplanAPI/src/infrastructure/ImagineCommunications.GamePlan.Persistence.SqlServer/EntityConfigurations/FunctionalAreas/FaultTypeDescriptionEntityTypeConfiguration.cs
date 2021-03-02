using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.FunctionalAreas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.FunctionalAreas
{
    public class FaultTypeDescriptionEntityTypeConfiguration : IEntityTypeConfiguration<FaultTypeDescription>
    {
        public void Configure(EntityTypeBuilder<FaultTypeDescription> builder)
        {
            builder.ToTable("FaultTypeDescriptions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).UseMySqlIdentityColumn();
            builder.Property(x => x.ShortName).HasMaxLength(100);
            builder.Property(x => x.Description).IsRequired();
            builder.Property(x => x.LanguageAbbreviation).IsRequired().HasMaxLength(3);

            builder.HasIndex(x => x.FaultTypeId);
        }
    }
}
