using ImagineCommunications.Gameplan.Integration.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ImagineCommunications.GamePlan.Integration.Data.Context.Configurations
{
    public class GroupTransactionInfoEntityConfiguration : IEntityTypeConfiguration<GroupTransactionInfo>
    {
        public void Configure(EntityTypeBuilder<GroupTransactionInfo> builder)
        {
            builder.ToTable("GroupTransactionInfos");
            builder.HasKey(x => x.Id);

            builder.HasMany(x => x.Messages).WithOne().HasForeignKey(x => x.GroupTransactionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
