using System;
using System.Collections.Generic;

namespace xggameplan.Updates
{
    /// <summary>
    /// Interface for an update, comprises of one or more update steps
    /// </summary>
    public interface IUpdate
    {
        /// <summary>
        /// Id
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Database version
        /// </summary>
        string DatabaseVersion { get; }

        /// <summary>
        /// Applies the update
        /// </summary>
        void Apply();

        /// <summary>
        /// Rolls back the update
        /// </summary>
        void RollBack();
        
        /// <summary>
        /// Whether update supports roll back
        /// </summary>
        bool SupportsRollback { get; }        

        /// <summary>
        /// Ids of updates that this update depends on
        /// </summary>
        List<Guid> DependsOnUpdates { get; }
    }
}
