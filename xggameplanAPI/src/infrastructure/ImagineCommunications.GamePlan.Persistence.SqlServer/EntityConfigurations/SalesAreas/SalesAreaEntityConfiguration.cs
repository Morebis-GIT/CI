using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.SalesAreas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.SalesAreas
{
    public class SalesAreaEntityConfiguration : IEntityTypeConfiguration<SalesArea>
    {
        public void Configure(EntityTypeBuilder<SalesArea> builder)
        {
            builder.ToTable("SalesAreas");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).HasDefaultValueSql("newid()");

            builder.Property(e => e.CustomId).Metadata.AfterSaveBehavior = PropertySaveBehavior.Ignore;
            builder.HasIndex(e => e.CustomId).IsUnique();
            builder.Property(e => e.Name).HasMaxLength(512);
            builder.HasIndex(e => e.Name).IsUnique();
            builder.Property(e => e.ShortName).HasMaxLength(256);
            builder.HasIndex(e => e.ShortName).IsUnique();
            builder.Property(e => e.CurrencyCode).HasMaxLength(64);
            builder.Property(e => e.TargetAreaName).HasMaxLength(512);
            builder.Property(e => e.BaseDemographic1).HasMaxLength(64);
            builder.HasIndex(e => e.BaseDemographic1);
            builder.Property(e => e.BaseDemographic2).HasMaxLength(64);
            builder.HasIndex(e => e.BaseDemographic2);

            builder.Property(e => e.DayDuration)
                   .HasConversion(
                        v => v.Ticks,
                        v => TimeSpan.FromTicks(v)
                    );

            builder.Property(e => e.StartOffset)
                   .HasConversion(
                        v => v.Ticks,
                        v => TimeSpan.FromTicks(v)
            );
            builder.HasMany(x => x.ChannelGroups).WithOne().HasForeignKey(x => x.SalesAreaId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.Holidays).WithOne().HasForeignKey(x => x.SalesAreaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
