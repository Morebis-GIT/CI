using System;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;
using xggameplan.Model;

namespace xggameplan.AutoBooks.Abstractions
{
    /// <summary>
    /// Interface for interacting with an AutoBook instance
    /// </summary>
    public interface IAutoBook
    {
        /// <summary>
        /// Starts AutoBook run
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="scenarioId"></param>
        /// <param name="real"></param>
        GetAutoBookStatusModel StartAutoBookRun(Guid runId, Guid scenarioId);

        /// <summary>
        /// Resets AutoBook for re-use
        /// </summary>
        void ResetFree();

        /// <summary>
        /// Deletes (cancels) AutoBook run
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="scenarioId"></param>
        void DeleteRun(Guid runId, Guid scenarioId);

        /// <summary>
        /// Gets AutoBook status
        /// </summary>
        /// <returns></returns>
        AutoBookStatuses GetStatus();

        /// <summary>
        /// Get AutoBook version
        /// </summary>
        /// <returns></returns>
        GetAutoBookVersionModel GetVersion();

        /// <summary>
        /// Gets audit trail for scenario
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        GetAutoBookAuditTrailModel GetAuditTrail(Guid scenarioId);

        /// <summary>
        /// Gets logs for scenario
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        GetAutoBookLogsModel GetLogs(Guid scenarioId);

        /// <summary>
        /// Gets snapshot for scenario
        /// </summary>
        /// <param name="scenarioId"></param>
        /// <returns></returns>
        GetAutoBookSnapshotModel GetSnapshot(Guid scenarioId);

        /// <summary>
        /// Gets storage info
        /// </summary>
        /// <returns></returns>
        GetAutoBookStorageInfoModel GetStorageInfo();
    }
}
