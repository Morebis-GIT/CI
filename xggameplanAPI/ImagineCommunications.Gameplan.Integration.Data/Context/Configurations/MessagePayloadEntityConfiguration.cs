using ImagineCommunications.Gameplan.Integration.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Integration.Data.Context.Configurations
{
    public class MessagePayloadEntityConfiguration : IEntityTypeConfiguration<MessagePayload>
    {
        public void Configure(EntityTypeBuilder<MessagePayload> builder)
        {
            builder.ToTable("MessagePayloads");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Payload).HasColumnType("VARBINARY(MAX)").IsRequired();
        }
    }
}
