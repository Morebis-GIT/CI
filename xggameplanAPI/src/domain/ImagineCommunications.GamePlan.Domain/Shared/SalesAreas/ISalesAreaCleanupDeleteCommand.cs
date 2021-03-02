using System;

namespace ImagineCommunications.GamePlan.Domain.Shared.SalesAreas
{
    /// <summary>
    /// An abstraction on the process of Sales Area deletion
    /// with references cleanup all around the system
    /// </summary>
    public interface ISalesAreaCleanupDeleteCommand
    {
        /// <summary>
        /// Delete Sales Area by its id
        /// along with some of its references
        /// all across the system
        /// </summary>
        void Execute(Guid salesAreaId);
    }
}
