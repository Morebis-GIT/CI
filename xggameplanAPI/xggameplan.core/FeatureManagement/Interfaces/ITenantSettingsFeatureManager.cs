using System;
using System.Collections.Generic;

namespace xggameplan.FeatureManagement
{
    /// <summary>
    /// Interface for managing features
    /// </summary>
    public interface ITenantSettingsFeatureManager
    {
        /// <summary>
        /// Gets Feature settings
        /// </summary>
        /// <param name="featureStr"></param>
        /// <returns>Dictionary object of features settings</returns>
        Dictionary<string, object> GetFeatureSettings(string featureStr);

        /// <summary>
        /// Gets Feature Enabled true/false flag
        /// </summary>
        /// <param name="featureStr"></param>
        /// <returns>True/False</returns>
        Boolean GetFeatureEnabled (string featureStr);
    }
}
