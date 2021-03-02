using ImagineCommunications.Gameplan.Integration.Data.Entities;
using ImagineCommunications.GamePlan.Integration.Data.Context.Configurations;
using Microsoft.EntityFrameworkCore;

namespace ImagineCommunications.Gameplan.Integration.Data.Context
{
    public class IntelligenceDbContext : DbContext
    {
        public DbSet<GroupTransactionInfo> GroupTransactionInfos { get; set; }
        public DbSet<MessageInfo> MessageInfos { get; set; }
        public DbSet<MessageType> MessageTypes { get; set; }
        public DbSet<MessagePayload> MessagePayloads { get; set; }
        public DbSet<MessageEntityType> MessageEntityTypes { get; set; }

        public IntelligenceDbContext(DbContextOptions<IntelligenceDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new GroupTransactionInfoEntityConfiguration());
            modelBuilder.ApplyConfiguration(new MessageInfoEntityConfiguration());
            modelBuilder.ApplyConfiguration(new MessageTypeEntityConfiguration());
            modelBuilder.ApplyConfiguration(new MessagePayloadEntityConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
