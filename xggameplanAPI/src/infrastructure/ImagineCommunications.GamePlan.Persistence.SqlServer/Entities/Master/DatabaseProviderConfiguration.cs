using ImagineCommunications.GamePlan.Domain.Shared.System;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Master
{
    public class DatabaseProviderConfiguration
    {
        public DbProviderType Provider { get;  set; }

        public string ConnectionString { get; set; }
    }
}
