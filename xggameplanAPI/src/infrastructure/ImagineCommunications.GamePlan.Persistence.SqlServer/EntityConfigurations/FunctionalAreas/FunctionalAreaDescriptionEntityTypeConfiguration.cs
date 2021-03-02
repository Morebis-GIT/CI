using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.FunctionalAreas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.FunctionalAreas
{
    public class FunctionalAreaDescriptionEntityTypeConfiguration : IEntityTypeConfiguration<FunctionalAreaDescription>
    {
        public void Configure(EntityTypeBuilder<FunctionalAreaDescription> builder)
        {
            builder.ToTable("FunctionalAreaDescriptions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Description).IsRequired();
            builder.Property(x => x.Id).UseSqlServerIdentityColumn();
            builder.Property(x => x.LanguageAbbreviation).IsRequired().HasMaxLength(3);

            builder.HasIndex(x => x.FunctionalAreaId);
        }
    }
}
