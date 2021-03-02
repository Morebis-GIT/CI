using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Programmes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Programmes
{
    public class ProgrammeCategoryLinkEntityConfiguration : IEntityTypeConfiguration<ProgrammeCategoryLink>
    {
        public void Configure(EntityTypeBuilder<ProgrammeCategoryLink> builder)
        {
            _ = builder.ToTable("ProgrammeCategoryLinks");

            _ = builder.HasKey(e => new { e.ProgrammeId, e.ProgrammeCategoryId });

            _ = builder.HasOne(x => x.Programme).WithMany(x => x.ProgrammeCategoryLinks)
                .HasForeignKey(x => x.ProgrammeId).OnDelete(DeleteBehavior.Cascade);
            _ = builder.HasOne(x => x.ProgrammeCategory).WithMany()
                .HasForeignKey(x => x.ProgrammeCategoryId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
