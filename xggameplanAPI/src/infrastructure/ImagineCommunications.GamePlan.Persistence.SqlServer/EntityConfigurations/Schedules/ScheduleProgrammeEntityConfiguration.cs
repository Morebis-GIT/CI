using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Programmes;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Schedules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Schedules
{
    public class ScheduleProgrammeEntityConfiguration : IEntityTypeConfiguration<ScheduleProgramme>
    {
        public void Configure(EntityTypeBuilder<ScheduleProgramme> builder)
        {
            _ = builder.ToTable("ScheduleProgrammes");
            _ = builder.HasKey(e => e.ProgrammeId);

            _ = builder.HasOne(x => x.Programme).WithOne()
                .HasForeignKey<ScheduleProgramme>(x => x.ProgrammeId)
                .HasPrincipalKey<Programme>(x => x.Id)
                .OnDelete(DeleteBehavior.Cascade);

            _ = builder.HasOne(x => x.Schedule).WithMany(x => x.Programmes)
                .OnDelete(DeleteBehavior.Cascade);

            _ = builder.HasIndex(e => e.ScheduleId)
                //.ForSqlServerInclude(nameof(ScheduleProgramme.ProgrammeId))
                .HasName("IX_ScheduleProgrammes_ScheduleId");
        }
    }
}
