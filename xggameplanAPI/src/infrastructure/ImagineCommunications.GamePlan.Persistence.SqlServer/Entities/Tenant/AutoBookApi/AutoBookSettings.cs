using System;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.AutoBookApi
{
    public class AutoBookSettings : IIdentityPrimaryKey
    {
        public int Id { get; set; }

        public bool Locked { get; set; }

        public string ProvisioningAPIURL { get; set; }

        public bool AutoProvisioning { get; set; }

        public DateTime AutoProvisioningLastActive { get; set; }

        public bool AutoDistributed { get; set; }

        public TimeSpan MinLifetime { get; set; }

        public TimeSpan MaxLifetime { get; set; }

        public TimeSpan CreationTimeout { get; set; }

        public int MinInstances { get; set; }

        public int MaxInstances { get; set; }

        public int SystemMaxInstances { get; set; }

        public string ApplicationVersion { get; set; }

        public string BinariesVersion { get; set; }
    }
}
