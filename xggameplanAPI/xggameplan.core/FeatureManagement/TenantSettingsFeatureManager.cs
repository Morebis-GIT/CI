using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;

namespace xggameplan.FeatureManagement
{
    public class TenantSettingsFeatureManager : ITenantSettingsFeatureManager
    {
        private readonly ITenantSettingsRepository _tenantSettingsRepository;

        public TenantSettingsFeatureManager(ITenantSettingsRepository tenantSettingsRepository)
        {
            _tenantSettingsRepository = tenantSettingsRepository;
        }

        public Dictionary<string, object> GetFeatureSettings(string featureStr)
        {
            Dictionary<string, object> featuresdict = new Dictionary<string, object>();
            var tenantSetting = _tenantSettingsRepository.Get();
            foreach (var feature in tenantSetting.Features)
            {
                if (feature.Id.ToLower() == featureStr.ToLower())
                {
                    foreach (KeyValuePair<string, object> setting in feature.Settings)
                    {
                        featuresdict.Add(setting.Key, setting.Value);
                    }
                }
            }

            return featuresdict;
        }

        public Boolean GetFeatureEnabled(string featureStr)
        {
            Boolean enabled = false;
            var tenantSetting = _tenantSettingsRepository.Get();
            foreach (var feature in tenantSetting.Features.ToArray())
            {
                if (feature.Id.ToLower() == featureStr.ToLower())
                {
                    enabled = feature.Enabled;
                }
            }

            return (enabled);
        }
    }
}
