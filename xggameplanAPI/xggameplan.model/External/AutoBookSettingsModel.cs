using System;
using NodaTime;

namespace xggameplan.Model
{
    public class AutoBookSettingsModel
    {
        public bool Locked { get; set; }
        public string ProvisioningAPIURL { get; set; }
        public bool AutoProvisioning { get; set; }
        public bool AutoDistributed { get; set; }
        public DateTime AutoProvisioningLastActive { get; set; }
        public Duration MinLifetime { get; set; }
        public Duration MaxLifetime { get; set; }
        public Duration CreationTimeout { get; set; }
        public int MinInstances { get; set; }
        public int MaxInstances { get; set; }
        public int SystemMaxInstances { get; set; }
        public string ApplicationVersion { get; set; }
        public string BinariesVersion { get; set; }
    }
}
