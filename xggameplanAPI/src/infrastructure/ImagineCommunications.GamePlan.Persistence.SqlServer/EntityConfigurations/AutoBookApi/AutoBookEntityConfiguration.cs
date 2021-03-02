using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations.AutoBookApi
{
    public class AutoBookEntityConfiguration : IEntityTypeConfiguration<AutoBook>
    {
        public void Configure(EntityTypeBuilder<AutoBook> builder)
        {
            builder.ToTable("AutoBooks");

            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).UseSqlServerIdentityColumn();

            builder.Property(e => e.AutoBookId).HasMaxLength(64).IsRequired();
            builder.Property(e => e.Api).HasMaxLength(256);

            builder.HasIndex(e => e.AutoBookId).IsUnique();

            builder.HasOne(e => e.Task).WithOne()
                .HasForeignKey<AutoBookTask>(e => e.AutoBookId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
