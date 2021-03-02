using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Restrictions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Restrictions
{
    public class RestrictionEntityConfiguration : IEntityTypeConfiguration<Restriction>
    {
        public void Configure(EntityTypeBuilder<Restriction> builder)
        {
            builder.ToTable("Restrictions");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseMySqlIdentityColumn();

            builder.Property(p => p.StartDate).AsUtc();
            builder.Property(p => p.EndDate).AsUtc();
            builder.Property(p => p.StartTime).AsTicks();
            builder.Property(p => p.EndTime).AsTicks();
            builder.Property(p => p.RestrictionDays).HasMaxLength(7);
            builder.Property(p => p.SchoolHolidayIndicator);
            builder.Property(p => p.PublicHolidayIndicator);
            builder.Property(p => p.LiveProgrammeIndicator);
            builder.Property(p => p.RestrictionType);
            builder.Property(p => p.RestrictionBasis);
            builder.Property(p => p.ExternalProgRef).HasMaxLength(64);
            builder.Property(p => p.ProgrammeCategory).HasMaxLength(64);
            builder.Property(p => p.ProgrammeClassification).HasMaxLength(64);
            builder.Property(p => p.ProgrammeClassificationIndicator);
            builder.Property(p => p.TimeToleranceMinsBefore);
            builder.Property(p => p.TimeToleranceMinsAfter);
            builder.Property(p => p.IndexType);
            builder.Property(p => p.IndexThreshold);
            builder.Property(p => p.ProductCode);
            builder.Property(p => p.ClashCode).HasMaxLength(64);
            builder.Property(p => p.ClearanceCode).HasMaxLength(64);
            builder.Property(p => p.ClockNumber).HasMaxLength(64).IsRequired();
            builder.Property(p => p.ExternalIdentifier).HasMaxLength(64);
            builder.Property(p => p.EpisodeNumber).HasDefaultValue(0);

            builder.HasIndex(p => p.ClashCode);
            builder.HasIndex(p => p.ClearanceCode);
            builder.HasIndex(p => p.ExternalProgRef);
            builder.HasIndex(p => p.ProgrammeCategory);
            builder.HasIndex(p => p.ProgrammeClassification);
            builder.HasIndex(p => p.Uid).IsUnique();
            builder.HasIndex(p => p.StartDate);
            builder.HasIndex(p => p.EndDate);
            builder.HasIndex(p => p.RestrictionType);

            builder.HasMany(e => e.SalesAreas).WithOne()
                .HasForeignKey(e => e.RestrictionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
