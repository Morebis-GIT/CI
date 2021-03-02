using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class ProgrammesClassificationEntityConfiguration
        : IEntityTypeConfiguration<ProgrammeClassification>
    {
        public void Configure(EntityTypeBuilder<ProgrammeClassification> builder)
        {
            builder.ToTable("ProgrammesClassifications");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.Description).HasMaxLength(512);
            builder.Property(e => e.Code).HasMaxLength(64);

            builder.HasIndex(e => e.Code);
            builder.HasIndex(e => e.Uid);
        }
    }
}
