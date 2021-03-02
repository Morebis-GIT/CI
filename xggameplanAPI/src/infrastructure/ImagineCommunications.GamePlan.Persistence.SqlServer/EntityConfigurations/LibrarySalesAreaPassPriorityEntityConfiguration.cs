using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class LibrarySalesAreaPassPriorityEntityConfiguration :
        AuditEntityTypeConfiguration<LibrarySalesAreaPassPriority>
    {
        public override void Configure(EntityTypeBuilder<LibrarySalesAreaPassPriority> builder)
        {
            builder.ToTable("LibrarySalesAreaPassPriorities");

            builder.HasKey(e => e.Uid);
            builder.Property(e => e.Uid);

            builder.Property(e => e.Id).HasDefaultValue(0)
                .Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;

            builder.Property(e => e.Name).HasMaxLength(255).IsRequired();
            builder.Property(e => e.StartTime).AsTicks();
            builder.Property(e => e.EndTime).AsTicks();
            builder.Property(e => e.DowPattern).AsStringPattern(DayOfWeek.Monday).IsRequired();

            builder.HasIndex(e => e.Name).IsUnique();

            builder.HasMany(e => e.SalesAreaPriorities).WithOne()
                .HasForeignKey(e => e.LibrarySalesAreaPassPriorityUid)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
