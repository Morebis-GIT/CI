using System;
using System.Collections.Generic;

namespace xggameplan.Updates
{
    /// <summary>
    /// Interface for update manager
    /// </summary>
    public interface IUpdateManager
    {
        /// <summary>
        /// Apply update
        /// </summary>
        /// <param name="updateId"></param>
        void Apply(Guid updateId);

        /// <summary>
        /// Roll back update
        /// </summary>
        /// <param name="updateId"></param>
        void RollBack(Guid updateId);

        /// <summary>
        /// Get update
        /// </summary>
        /// <param name="updateId"></param>
        /// <returns></returns>
        IUpdate GetUpdate(Guid updateId);

        /// <summary>
        /// Returns all updates needed to update from old database version to new database version
        /// </summary>
        /// <param name="oldDatabaseVersion"></param>
        /// <param name="newDatabaseVersion"></param>
        /// <returns></returns>
        List<Guid> GetUpdates(string oldDatabaseVersion, string newDatabaseVersion);
    }
}
