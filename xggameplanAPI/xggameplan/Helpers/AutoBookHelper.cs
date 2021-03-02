using ImagineCommunications.GamePlan.Domain.AutoBookApi.Settings;
using NodaTime;

namespace xggameplan.Helpers
{
    public static class AutoBookHelper
    {
        /// <summary>
        /// Returns default AutoBookSettings for the system.
        /// Auto-provisioning is disabled so that the setting can be reviewed before we turn it on.
        /// </summary>
        /// <returns></returns>
        public static AutoBookSettings GetDefaultAutoBookSettings()
        {
            AutoBookSettings autoBookSettings = new AutoBookSettings()
            {
                AutoProvisioning = false,           // Disabled so that we don't start re-provisioning with the version below
                MinInstances = 0,                   // We only want instances when we need them, reduces costs
                MaxInstances = 5,
                SystemMaxInstances = 10,             // Low limit to start with
                CreationTimeout = Duration.FromMinutes(15),   // Takes about 5 mins usually
                MinLifetime = Duration.FromMinutes(65),       // At least 1 hr
                MaxLifetime = Duration.FromMilliseconds(0),   // No need to recycle
                ProvisioningAPIURL = "http://xg-autobook-provider-api-dev.eu-west-2.elasticbeanstalk.com",     // Same API for all systems
                ApplicationVersion = null,         // "v1.53.0-dev"
                BinariesVersion = null,
                Locked = false
            };

            return autoBookSettings;
        }
    }
}
