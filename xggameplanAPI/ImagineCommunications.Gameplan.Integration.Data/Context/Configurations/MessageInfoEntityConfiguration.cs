using ImagineCommunications.Gameplan.Integration.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Integration.Data.Context.Configurations
{
    public class MessageInfoEntityConfiguration : IEntityTypeConfiguration<MessageInfo>
    {
        public void Configure(EntityTypeBuilder<MessageInfo> builder)
        {
            builder.ToTable("MessageInfos");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();

            builder
                .HasOne<MessagePayload>()
                .WithOne()
                .HasForeignKey<MessagePayload>(x => x.Id);
            builder.HasOne<GroupTransactionInfo>().WithMany(x => x.Messages).HasForeignKey(x => x.GroupTransactionId);

            builder.HasIndex(x => x.GroupTransactionId);
        }
    }
}
