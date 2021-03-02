using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public virtual void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(k => k.Id);
            builder.Property(k => k.Id).UseSqlServerIdentityColumn();

            builder.Property(e => e.Name).IsRequired().HasMaxLength(128);
            builder.Property(e => e.Surname).IsRequired().HasMaxLength(128);
            builder.Property(e => e.Email).IsRequired().HasMaxLength(128);
            builder.Property(e => e.Password).HasMaxLength(256);
            builder.Property(e => e.Role).HasMaxLength(128);
            builder.Property(e => e.ThemeName).HasMaxLength(128).IsRequired();
            builder.Property(e => e.Location).HasMaxLength(256);
            builder.Property(e => e.DefaultTimeZone).IsRequired().HasMaxLength(128);

            builder.HasIndex(e => e.Email).IsUnique();

            builder.Property(e => e.TenantId).IsRequired();
            builder.HasOne(e => e.Preview).WithOne()
                .HasForeignKey<Entities.Master.PreviewFile>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.UserSettings).WithOne()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
