using System;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Tenants
{
    public class ProviderConfiguration
    {
        public string Provider { get; set; }
        public string ConfigurationJson { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is ProviderConfiguration providerConfiguration)
            {
                return Equals(providerConfiguration);
            }

            return false;
        }

        protected bool Equals(ProviderConfiguration other)
        {
            return string.Equals(Provider, other.Provider, StringComparison.InvariantCulture) &&
                   string.Equals(ConfigurationJson, other.ConfigurationJson, StringComparison.InvariantCulture);
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }
}
