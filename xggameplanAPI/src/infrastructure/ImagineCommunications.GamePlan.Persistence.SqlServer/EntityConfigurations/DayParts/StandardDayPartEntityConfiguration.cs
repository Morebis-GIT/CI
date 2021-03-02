using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.DayParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.DayParts
{
    public class StandardDayPartEntityConfiguration : IEntityTypeConfiguration<StandardDayPart>
    {
        public void Configure(EntityTypeBuilder<StandardDayPart> builder)
        {
            builder.ToTable("StandardDayParts");
            builder.HasKey(k => k.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();
            builder.Property(e => e.SalesArea).HasMaxLength(512).IsRequired();
            builder.Property(e => e.Name).HasMaxLength(64).IsRequired();

            builder.HasMany(x => x.Timeslices).WithOne()
                .HasForeignKey(x => x.DayPartId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
