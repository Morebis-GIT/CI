using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;

namespace xggameplan.core.FeatureManagement.Interfaces
{
    /// <summary>
    /// Manages features storage
    /// </summary>
    public interface IFeatureSettingsProvider
    {
        /// <summary>
        /// Gets all available features for the specified tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        IEnumerable<IFeatureSetting> GetForTenant(int tenantId);
    }
}
