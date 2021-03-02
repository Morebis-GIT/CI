using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Passes
{
    public class PassEntityConfiguration : IEntityTypeConfiguration<Pass>
    {
        public void Configure(EntityTypeBuilder<Pass> builder)
        {
            builder.ToTable("Passes");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name).HasMaxLength(TextColumnLenght.Normal);
            builder.Property(e => e.DateCreated).AsUtc();
            builder.Property(e => e.DateModified).AsUtc();

            builder.Property<string>(Pass.SearchField).HasMaxLength(300);
            builder.HasOne(x => x.PassSalesAreaPriorities).WithOne()
                .HasForeignKey<PassSalesAreaPriorityCollection>(x => x.PassId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.General).WithOne().HasForeignKey(x => x.PassId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Weightings).WithOne().HasForeignKey(x => x.PassId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Tolerances).WithOne().HasForeignKey(x => x.PassId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Rules).WithOne().HasForeignKey(x => x.PassId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.RatingPoints).WithOne().HasForeignKey(x => x.PassId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(x => x.ProgrammeRepetitions).WithOne().HasForeignKey(x => x.PassId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.BreakExclusions).WithOne().HasForeignKey(x => x.PassId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.SlottingLimits).WithOne().HasForeignKey(x => x.PassId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.Name);
            builder.HasIndex(x => x.IsLibraried);
        }
    }
}
