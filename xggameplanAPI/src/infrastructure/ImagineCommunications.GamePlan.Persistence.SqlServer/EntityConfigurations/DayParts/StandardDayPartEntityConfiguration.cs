using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.DayParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.DayParts
{
    public class StandardDayPartEntityConfiguration : IEntityTypeConfiguration<StandardDayPart>
    {
        public void Configure(EntityTypeBuilder<StandardDayPart> builder)
        {
            _ = builder.ToTable("StandardDayParts");
            _ = builder.HasKey(k => k.Id);
            _ = builder.Property(e => e.Id).UseSqlServerIdentityColumn();
            _ = builder.HasOne(x => x.SalesArea)
                .WithMany()
                .HasForeignKey(x => x.SalesAreaId)
                .OnDelete(DeleteBehavior.Restrict);
            _ = builder.Property(e => e.Name).HasMaxLength(64).IsRequired();

            _ = builder.HasMany(x => x.Timeslices).WithOne()
                .HasForeignKey(x => x.DayPartId)
                .OnDelete(DeleteBehavior.Cascade);
            _ = builder.HasIndex(x => x.SalesAreaId);
        }
    }
}
