using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Shared.System.Settings
{
    /// <summary>
    /// Product feature
    /// </summary>
    public class Feature
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Whether feature is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Settings for feature
        /// </summary>
        public Dictionary<string, object> Settings = new Dictionary<string, object>();
    }
}
