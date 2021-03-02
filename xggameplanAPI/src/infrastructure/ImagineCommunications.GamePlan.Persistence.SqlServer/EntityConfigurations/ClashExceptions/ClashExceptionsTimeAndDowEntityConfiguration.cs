using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.ClashExceptions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.ClashExceptions
{
    public class ClashExceptionsTimeAndDowEntityConfiguration : IEntityTypeConfiguration<ClashExceptionsTimeAndDow>
    {
        public void Configure(EntityTypeBuilder<ClashExceptionsTimeAndDow> builder)
        {
            builder.ToTable("ClashExceptionsTimeAndDows");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();
            builder.HasIndex(e => e.ClashExceptionId);
            builder.Property(e => e.DaysOfWeek).HasMaxLength(32);
            builder.Property(e => e.EndTime).AsTicks();
            builder.Property(e => e.StartTime).AsTicks();
        }
    }
}
