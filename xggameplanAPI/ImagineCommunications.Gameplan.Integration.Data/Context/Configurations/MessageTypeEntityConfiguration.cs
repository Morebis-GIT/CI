using ImagineCommunications.Gameplan.Integration.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Integration.Data.Context.Configurations
{
    public class MessageTypeEntityConfiguration : IEntityTypeConfiguration<MessageType>
    {
        public void Configure(EntityTypeBuilder<MessageType> builder)
        {
            builder.ToTable("MessageTypes");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .HasMaxLength(64)
                .IsRequired();

            builder.Property(x => x.Name)
                .HasMaxLength(256)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasMaxLength(512);

            builder.HasOne(c => c.MessageEntityType)
                .WithMany(m => m.MessageTypes)
                .HasForeignKey(c => c.MessageEntityTypeId);

            builder.Property(x => x.Priority).IsRequired();
        }
    }
}
