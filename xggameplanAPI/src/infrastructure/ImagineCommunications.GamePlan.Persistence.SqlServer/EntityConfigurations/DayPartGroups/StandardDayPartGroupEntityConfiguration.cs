using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.DayParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.DayPartGroups
{
    public class StandardDayPartGroupEntityConfiguration : IEntityTypeConfiguration<StandardDayPartGroup>
    {
        public void Configure(EntityTypeBuilder<StandardDayPartGroup> builder)
        {
            _ = builder.ToTable("StandardDayPartGroups");
            _ = builder.HasKey(k => k.Id);
            _ = builder.Property(e => e.Id).UseSqlServerIdentityColumn();
            _ = builder.HasOne(x => x.SalesArea)
                .WithMany()
                .HasForeignKey(x => x.SalesAreaId)
                .OnDelete(DeleteBehavior.Restrict);
            _ = builder.Property(e => e.Demographic).HasMaxLength(64).IsRequired();

            _ = builder.HasMany(x => x.Splits).WithOne()
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
            _ = builder.HasIndex(x => x.SalesAreaId);
        }
    }
}
