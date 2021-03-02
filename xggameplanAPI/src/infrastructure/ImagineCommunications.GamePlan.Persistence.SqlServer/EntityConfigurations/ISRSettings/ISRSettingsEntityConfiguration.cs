using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.ISRSettings
{
    public class ISRSettingsEntityConfiguration : IEntityTypeConfiguration<Entities.Tenant.ISRSettings.ISRSettings>
    {
        public void Configure(EntityTypeBuilder<Entities.Tenant.ISRSettings.ISRSettings> builder)
        {
            _ = builder.ToTable("ISRSettings");

            _ = builder.HasKey(e => e.Id);
            _ = builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            _ = builder.Property(e => e.DefaultEfficiencyThreshold);
            _ = builder.Property(e => e.BreakType).HasMaxLength(64);
            _ = builder.Property(e => e.StartTime).AsTicks();
            _ = builder.Property(e => e.EndTime).AsTicks();
            _ = builder.Property(e => e.ExcludePublicHolidays);
            _ = builder.Property(e => e.ExcludeSchoolHolidays);
            _ = builder.Property(e => e.SelectableDays).AsStringPattern(DayOfWeek.Sunday);

            _ = builder.HasMany(e => e.DemographicsSettings).WithOne()
                .HasForeignKey(e => e.ISRSettingId)
                .OnDelete(DeleteBehavior.Cascade);
            _ = builder.HasOne(e => e.SalesArea).WithMany()
                .HasForeignKey(e => e.SalesAreaId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = builder.HasIndex(x => x.SalesAreaId);
        }
    }
}
