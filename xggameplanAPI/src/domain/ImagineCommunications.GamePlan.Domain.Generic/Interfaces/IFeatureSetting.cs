using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.Generic.Interfaces
{
    public interface IFeatureSetting
    {
        int Id { get; }

        string Name { get; }

        /// <summary>
        /// Indicator whether this feature can be sent to client applications
        /// </summary>
        bool IsShared { get; }

        /// <summary>
        /// Feature state
        /// </summary>
        bool Enabled { get; }

        IReadOnlyCollection<int> ParentIds { get; }
    }
}
