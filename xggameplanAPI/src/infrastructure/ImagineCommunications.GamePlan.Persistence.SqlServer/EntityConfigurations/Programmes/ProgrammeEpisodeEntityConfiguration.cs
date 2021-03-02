using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Programmes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.Programmes
{
    public class ProgrammeEpisodeEntityConfiguration : IEntityTypeConfiguration<ProgrammeEpisode>
    {
        public void Configure(EntityTypeBuilder<ProgrammeEpisode> builder)
        {
            _ = builder.ToTable("ProgrammeEpisodes");
            _ = builder.HasKey(e => e.Id);
            _ = builder.Property(e => e.Id).UseMySqlIdentityColumn();
            _ = builder.Property(x => x.Name).HasMaxLength(64).IsRequired();

            _ = builder.HasOne(x => x.ProgrammeDictionary).WithMany(x => x.ProgrammeEpisodes)
                .HasForeignKey(x => x.ProgrammeDictionaryId).OnDelete(DeleteBehavior.Cascade);

            _ = builder.HasIndex(p => new { p.ProgrammeDictionaryId });
        }
    }
}
