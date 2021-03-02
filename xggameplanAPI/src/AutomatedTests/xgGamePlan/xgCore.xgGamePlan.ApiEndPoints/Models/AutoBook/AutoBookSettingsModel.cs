using System;
using System.Collections.Generic;
using NodaTime;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.AutoBook
{
    public class AutoBookSettingsModel
    {
        public bool Locked { get; set; }
        public string ProvisioningAPIURL { get; set; }
        public bool AutoProvisioning { get; set; }
        public DateTime AutoProvisioningLastActive { get; set; }
        public Duration MinLifetime { get; set; }
        public Duration MaxLifetime { get; set; }
        public Duration CreationTimeout { get; set; }
        public int MinInstances { get; set; }
        public int MaxInstances { get; set; }
        public int SystemMaxInstances { get; set; }
        public string ApplicationVersion { get; set; }
        public string BinariesVersion { get; set; }

        public override bool Equals(object obj)
        {
            var model = obj as AutoBookSettingsModel;
            return model != null &&
                   Locked == model.Locked &&
                   ProvisioningAPIURL == model.ProvisioningAPIURL &&
                   AutoProvisioning == model.AutoProvisioning &&
                   AutoProvisioningLastActive == model.AutoProvisioningLastActive &&
                   MinLifetime.Equals(model.MinLifetime) &&
                   MaxLifetime.Equals(model.MaxLifetime) &&
                   CreationTimeout.Equals(model.CreationTimeout) &&
                   MinInstances == model.MinInstances &&
                   MaxInstances == model.MaxInstances &&
                   SystemMaxInstances == model.SystemMaxInstances &&
                   ApplicationVersion == model.ApplicationVersion &&
                   BinariesVersion == model.BinariesVersion;
        }

        public override int GetHashCode()
        {
            int hashCode = 1745651570;
            hashCode = hashCode * -1521134295 + Locked.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ProvisioningAPIURL);
            hashCode = hashCode * -1521134295 + AutoProvisioning.GetHashCode();
            hashCode = hashCode * -1521134295 + AutoProvisioningLastActive.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Duration>.Default.GetHashCode(MinLifetime);
            hashCode = hashCode * -1521134295 + EqualityComparer<Duration>.Default.GetHashCode(MaxLifetime);
            hashCode = hashCode * -1521134295 + EqualityComparer<Duration>.Default.GetHashCode(CreationTimeout);
            hashCode = hashCode * -1521134295 + MinInstances.GetHashCode();
            hashCode = hashCode * -1521134295 + MaxInstances.GetHashCode();
            hashCode = hashCode * -1521134295 + SystemMaxInstances.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ApplicationVersion);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(BinariesVersion);
            return hashCode;
        }
    }
}
