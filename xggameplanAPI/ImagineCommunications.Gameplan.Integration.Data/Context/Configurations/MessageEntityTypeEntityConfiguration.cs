using ImagineCommunications.Gameplan.Integration.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.Gameplan.Integration.Data.Context.Configurations
{
    public class MessageEntityTypeEntityConfiguration : IEntityTypeConfiguration<MessageEntityType>
    {
        public void Configure(EntityTypeBuilder<MessageEntityType> builder)
        {
            builder.ToTable("MessageEntityTypes");

            builder.HasKey(x => x.Id);

            builder.Property(n => n.Name)
                .HasMaxLength(128)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasMaxLength(512);
        }
    }
}
