using System.Collections.Generic;

namespace xggameplan.core.FeatureManagement.Interfaces
{
    /// <summary>
    /// Manages features state
    /// </summary>
    public interface IFeatureManager
    {
        /// <summary>
        /// Gets global product feature enabled state of current tenant
        /// </summary>
        /// <param name="featureName"></param>
        /// <returns>feature state</returns>
        bool IsEnabled(string featureName);

        IReadOnlyCollection<IFeatureFlag> Features { get; }

        void ClearCache();
    }
}
