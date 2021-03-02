using System;
using NodaTime;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.Settings
{
    /// <summary>
    /// AutoBook settings of which should only be one instance
    /// </summary>
    public class AutoBookSettings
    {
        public int Id { get; set; }

        /// <summary>
        /// Indicates whether these settings can be modified. Applying the provisioning settings (E.g. Adding/Deleting AutoBook instances) will
        /// take time and so we don't want settings to modified again until the settings have been applied.
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// URL for Provisioning API, managing environments, AutoBooks etc
        /// </summary>
        public string ProvisioningAPIURL { get; set; }

        /// <summary>
        /// Whether to auto-provision instances, otherwise manual.
        /// </summary>
        public bool AutoProvisioning { get; set; }

        /// <summary>
        /// Time that auto-provisioning was last active
        /// </summary>
        public DateTime AutoProvisioningLastActive { get; set; }

        /// <summary>
        /// Distributed flag
        /// </summary>
        public bool AutoDistributed { get; set; }

        /// <summary>
        /// Minimum lifetime for instance. This is necessary because AWS will charge for 1 hr even if instance is only used for
        /// a few mins. We should therefore keep the instance for the full hour. Will be zero if no minimum lifetime.
        /// </summary>
        public Duration MinLifetime { get; set; }

        /// <summary>
        /// Maximum lifetime for instance. This is for recycling instances. Will be zero if no recycling required.
        /// </summary>
        public Duration MaxLifetime { get; set; }

        /// <summary>
        /// Timeout for AutoBook creation. This property is used for identifying issues creating AutoBook instances.
        /// </summary>
        public Duration CreationTimeout { get; set; }

        /// <summary>
        /// Min number of AutoBook instances that should be provisioned (0=No minimum). If no minumum defined then AutoBook instance
        /// will be unprovisioned after MinLifetime or MaxLifetime is reached.
        /// </summary>
        public int MinInstances { get; set; }

        /// <summary>
        /// Max number of AutoBook instances that can be provisioned
        /// </summary>
        public int MaxInstances { get; set; }

        /// <summary>
        /// System limit for MaxInstances
        /// </summary>
        public int SystemMaxInstances { get; set; }

        /// <summary>
        /// Version of AutoBook API that should be used.
        /// </summary>
        public string ApplicationVersion { get; set; }

        /// <summary>
        /// Version of Optimiser binaries that should be used.
        /// </summary>
        public string BinariesVersion { get; set; }

        // Default timeouts, these are only used in system tests
        public static int CreateAutoBookTimeoutSeconds = 300;
        public static int RestartAutoBookTimeoutSeconds = 300;

        /// <summary>
        /// Storage in GB for 1 million breaks (200,000 breaks=2GB)
        /// </summary>
        public static double StorageGBPerMillionBreaks = 10;

        public static double UnknownStorageGB = -1;

        /// <summary>
        /// Updates properties
        /// </summary>
        /// <param name="provisioningAPIURL"></param>
        /// <param name="autoProvisioning"></param>
        /// <param name="minLifetime"></param>
        /// <param name="maxLifetime"></param>
        /// <param name="creationTimeout"></param>
        /// <param name="minInstances"></param>
        /// <param name="maxInstances"></param>
        /// <param name="applicationVersion"></param>
        public void Update(string provisioningAPIURL,
                            bool autoProvisioning,
                            bool autoDistributed,
                            Duration minLifetime,
                            Duration maxLifetime,
                            Duration creationTimeout,
                            int minInstances,
                            int maxInstances,
                            string applicationVersion,
                            string binariesVersion)
        {
            // Validate
            if (String.IsNullOrEmpty(provisioningAPIURL))
            {
                throw new Exception("Provisioning API URL is not set");
            }
            if (minInstances != 0)    // Now that we set instance configuration then we need a specific scenario run to create instances
            {
                throw new Exception("Minimum instances must be zero");
            }
            if (maxInstances < 1)
            {
                throw new Exception("Maximum Instances must be 1 or more");
            }
            if (minInstances > 0 && maxInstances > 0 && minInstances > maxInstances)
            {
                throw new Exception("Minimum Instances must be less than Maximum Instances");
            }
            if (maxInstances > SystemMaxInstances)
            {
                throw new Exception(String.Format("Maximum Instances cannot be greater than the system limit of {0}", SystemMaxInstances));
            }
            if (creationTimeout.ToTimeSpan().TotalMilliseconds <= 0)
            {
                throw new Exception("Creation Timeout must be set");
            }

            ProvisioningAPIURL = provisioningAPIURL;
            AutoProvisioning = autoProvisioning;
            AutoDistributed = autoDistributed;
            MinLifetime = minLifetime;
            MaxLifetime = maxLifetime;
            CreationTimeout = creationTimeout;
            MinInstances = minInstances;
            MaxInstances = maxInstances;
            ApplicationVersion = applicationVersion;
            BinariesVersion = binariesVersion;
        }
    }
}
