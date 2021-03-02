using System.Collections.Generic;

namespace xggameplan.core.FeatureManagement.Interfaces
{
    public interface IFeatureFlag
    {
        /// <summary>
        /// Name of the feature
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Indicator whether this feature can be sent to client applications
        /// </summary>
        bool IsShared { get; }

        /// <summary>
        /// Evaluated feature state
        /// </summary>
        bool IsEnabled { get; }

        IReadOnlyCollection<IFeatureFlag> DependsOn { get; }
    }
}
