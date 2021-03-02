using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.EntityConfigurations
{
    public class UserSettingEntityConfiguration : IEntityTypeConfiguration<UserSetting>
    {
        public virtual void Configure(EntityTypeBuilder<UserSetting> builder)
        {
            builder.ToTable("UserSettings");

            builder.HasKey(k => k.Id);
            builder.Property(k => k.Id).UseSqlServerIdentityColumn();

            builder.Property(e => e.Name).IsRequired().HasMaxLength(128);
            builder.Property(e => e.Value).HasColumnType("NVARCHAR(MAX)");
            builder.Property(e => e.UserId).IsRequired();

            builder.HasIndex(e => e.UserId);
            builder.HasIndex(e => e.Name);
        }
    }
}
