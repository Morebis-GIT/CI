using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class TenantEntityConfiguration : IEntityTypeConfiguration<Tenant>
    {
        public virtual void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.ToTable("Tenants");

            builder.HasKey(k => k.Id);
            builder.Property(p => p.Id).UseMySqlIdentityColumn();

            builder.Property(e => e.Name).HasMaxLength(128).IsRequired();
            builder.Property(e => e.DefaultTheme).HasMaxLength(128);

            builder.OwnsOne(p => p.TenantDb, build =>
            {
                build.Property(t => t.ConnectionString)
                    .HasMaxLength(256)
                    .IsRequired();
            });

            builder.HasOne(e => e.Preview).WithOne()
               .HasForeignKey<PreviewFile>(e => e.TenantId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
