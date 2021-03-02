using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.DayParts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.DayPartGroups
{
    public class StandardDayPartGroupEntityConfiguration : IEntityTypeConfiguration<StandardDayPartGroup>
    {
        public void Configure(EntityTypeBuilder<StandardDayPartGroup> builder)
        {
            builder.ToTable("StandardDayPartGroups");
            builder.HasKey(k => k.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();
            builder.Property(e => e.SalesArea).HasMaxLength(512).IsRequired();
            builder.Property(e => e.Demographic).HasMaxLength(64).IsRequired();

            builder.HasMany(x => x.Splits).WithOne()
                .HasForeignKey(x => x.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
