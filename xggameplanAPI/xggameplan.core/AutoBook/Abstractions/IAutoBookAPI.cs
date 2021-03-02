using System;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;
using xggameplan.Model;

namespace xggameplan.AutoBooks.Abstractions
{
    /// <summary>
    /// Interface for communication with AutoBook instance
    /// </summary>
    public interface IAutoBookAPI
    {
        /// <summary>
        /// Starts run, returns status which should In_Progress
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="scenarioId"></param>
        /// <param name="real"></param>
        GetAutoBookStatusModel StartAutoBookRun(Guid runId, Guid scenarioId);

        /// <summary>
        /// Deletes run
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="scenarioId"></param>
        void DeleteRun(Guid runId, Guid scenarioId);

        /// <summary>
        /// Gets snapshot
        /// </summary>
        /// <param name="scenarioId"></param>
        GetAutoBookSnapshotModel GetSnapshot(Guid scenarioId);

        /// <summary>
        /// Obtains AutoBook status
        /// </summary>
        /// <returns></returns>
        AutoBookStatuses GetStatus();

        /// <summary>
        /// Gets AutoBook version
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
        /// Gets storage info
        /// </summary>
        /// <returns></returns>
        GetAutoBookStorageInfoModel GetStorageInfo();
    }
}
