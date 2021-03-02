using Microsoft.EntityFrameworkCore;
using ImagineCommunications.Gameplan.Synchronization.SqlServer.Entities;
using ImagineCommunications.Gameplan.Synchronization.SqlServer.Context.Configurations;

namespace ImagineCommunications.Gameplan.Synchronization.SqlServer.Context
{
    public class SynchronizationDbContext : DbContext
    {
        public DbSet<SynchronizationObject> Objects { get; set; }

        public DbSet<SynchronizationObjectOwner> ObjectOwners { get; set; }

        public SynchronizationDbContext(DbContextOptions<SynchronizationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new SynchronizationObjectEntityConfiguration());
            modelBuilder.ApplyConfiguration(new SynchronizationObjectOwnerEntityConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
