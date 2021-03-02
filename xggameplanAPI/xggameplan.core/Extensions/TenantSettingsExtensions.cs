using ImagineCommunications.GamePlan.Domain.Generic.Types;
using ImagineCommunications.GamePlan.Domain.Shared.System.Tenants;
using xggameplan.core.FeatureManagement.Interfaces;

namespace xggameplan.core.Extensions
{
    public static class TenantSettingsExtensions
    {
        public static TenantTimeSettings ConvertToTimeTenantSettings(this TenantSettings tenantSettings, IFeatureManager featureManager)
        {
            var useSystemLogicalDate = featureManager.IsEnabled(nameof(ProductFeature.UseSystemLogicalDate));

            var tenantTimeSettings = new TenantTimeSettings
            {
                MidnightStartTime = tenantSettings.MidnightStartTime,
                MidnightEndTime = tenantSettings.MidnightEndTime,
                PeakStartTime = tenantSettings.PeakStartTime,
                PeakEndTime = tenantSettings.PeakEndTime,
                StartDayOfWeek = tenantSettings.StartDayOfWeek,
            };

            if (useSystemLogicalDate)
            {
                tenantTimeSettings.SystemLogicalDate = tenantSettings.SystemLogicalDate;
            }

            return tenantTimeSettings;
        }
    }
}
