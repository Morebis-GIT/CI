using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class ClearanceCodeEntityConfiguration : IEntityTypeConfiguration<ClearanceCode>
    {
        public void Configure(EntityTypeBuilder<ClearanceCode> builder)
        {
            builder.ToTable("ClearanceCodes");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.Code).HasMaxLength(64);
            builder.Property(e => e.Description).HasMaxLength(512);
        }
    }
}
