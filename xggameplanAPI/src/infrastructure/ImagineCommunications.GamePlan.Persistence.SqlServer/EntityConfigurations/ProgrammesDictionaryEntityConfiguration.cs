using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class ProgrammesDictionaryEntityConfiguration : IEntityTypeConfiguration<ProgrammeDictionary>
    {
        public void Configure(EntityTypeBuilder<ProgrammeDictionary> builder)
        {
            _ = builder.ToTable("ProgrammeDictionaries");

            _ = builder.HasKey(e => e.Id);

            _ = builder.Property(e => e.Id).UseSqlServerIdentityColumn();
            _ = builder.Property(e => e.ExternalReference).HasMaxLength(64).IsRequired();
            _ = builder.Property(e => e.Name).HasMaxLength(128).IsRequired();
            _ = builder.Property(e => e.Description).HasMaxLength(512);
            _ = builder.Property(e => e.Classification).HasMaxLength(64);
            _ = builder.Property<string>(ProgrammeDictionary.SearchField)
                .HasComputedColumnSql(
                    $"CONCAT_WS(' ', {nameof(ProgrammeDictionary.ExternalReference)}, {nameof(ProgrammeDictionary.Name)})");

            _ = builder.HasMany(x => x.ProgrammeEpisodes).WithOne(x => x.ProgrammeDictionary)
                .OnDelete(DeleteBehavior.Cascade);

            _ = builder.HasIndex(e => e.ExternalReference).IsUnique();
        }

    }
}
