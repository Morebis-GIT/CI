using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ClashExceptions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.ClashExceptions
{
    public class ClashExceptionEntityConfiguration : IEntityTypeConfiguration<ClashException>
    {
        public void Configure(EntityTypeBuilder<ClashException> builder)
        {
            builder.ToTable("ClashExceptions");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            builder.Property(e => e.FromType);
            builder.Property(e => e.FromValue).HasMaxLength(64);
            builder.Property(e => e.ToType);
            builder.Property(e => e.ToValue).HasMaxLength(64);
            builder.Property(e => e.StartDate).AsUtc().IsRequired();
            builder.Property(e => e.EndDate).AsUtc();
            builder.Property(e => e.ExternalRef).HasMaxLength(64);

            builder.HasMany(e => e.ClashExceptionsTimeAndDows).WithOne()
                .HasForeignKey(e => e.ClashExceptionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
