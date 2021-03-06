﻿using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class ClashEntityTypeConfiguration : IEntityTypeConfiguration<Clash>
    {
        public void Configure(EntityTypeBuilder<Clash> builder)
        {
            builder.ToTable("Clashes");

            builder.HasKey(e => e.Uid);
            builder.Property(e => e.Uid).HasDefaultValueSql("newid()");

            builder.Property(p => p.Externalref).HasMaxLength(64);
            builder.Property(p => p.ParentExternalidentifier).HasMaxLength(64);
            builder.Property(p => p.Description).HasMaxLength(512);

            builder.Property<string>(Clash.SearchField).HasComputedColumnSql("CONCAT_WS(' ', Externalref, Description)");

            builder.HasMany(e => e.Differences).WithOne().HasForeignKey(e => e.ClashId);

            builder.HasIndex(e => e.Externalref);
            builder.HasIndex(e => e.Description);
            builder.HasIndex(e => e.ParentExternalidentifier);
        }
    }
}
