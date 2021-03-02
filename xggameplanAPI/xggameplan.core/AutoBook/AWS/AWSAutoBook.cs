using System;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects;
using xggameplan.AutoBooks.Abstractions;
using xggameplan.Model;

namespace xggameplan.AutoBooks.AWS
{
    /// <summary>
    /// For managing and interacting with an AWS AutoBook instance.
    /// It includes provisioning, input data upload, AutoBook REST API interaction, output data download
    /// </summary>
    public class AWSAutoBook : IAutoBook
    {
        private AutoBook _autoBook;
        private IAutoBookRepository _autoBookRepository;
        private IAutoBookAPI _autoBookApi = null;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="autoBook">AutoBool details</param>
        /// <param name="autoBookRepository">AutoBook repository</param>
        /// <param name="scenarioResultRepository">ScenarioResult repository</param>
        /// <param name="outputFileRepository"></param>
        /// <param name="autoBookApi"></param>
        /// <param name="mapper">Mapper</param>
        /// <param name="resultsFilterRepository"></param>
        public AWSAutoBook(AutoBook autoBook,
                           IAutoBookRepository autoBookRepository,
                           IAutoBookAPI autoBookApi)
        {
            _autoBook = autoBook;
            _autoBookRepository = autoBookRepository;
            _autoBookApi = autoBookApi;
        }

        /// <summary>
        /// Starts run of scenario
        /// </summary>
        /// <param name="runId"></param>
        /// <param name="scenarioId"></param>
        /// <param name="real"></param>
        public GetAutoBookStatusModel StartAutoBookRun(Guid runId, Guid scenarioId)
        {
            return _autoBookApi.StartAutoBookRun(runId, scenarioId);
        }

        /// <summary>
        /// Resets AutoBook to be free for re-use
        /// </summary>
        public void ResetFree()
        {
            lock (_autoBook)
            {
                _autoBook.Locked = false;
                _autoBook.Task = null;
                _autoBookRepository.Update(_autoBook);
            }

        }

        public AutoBookStatuses GetStatus()
        {
            return _autoBookApi.GetStatus();
        }

        public GetAutoBookVersionModel GetVersion()
        {
            return _autoBookApi.GetVersion();
        }

        public GetAutoBookAuditTrailModel GetAuditTrail(Guid scenarioId)
        {
            return _autoBookApi.GetAuditTrail(scenarioId);
        }

        public GetAutoBookLogsModel GetLogs(Guid scenarioId)
        {
            return _autoBookApi.GetLogs(scenarioId);
        }

        public GetAutoBookSnapshotModel GetSnapshot(Guid scenarioId)
        {
            return _autoBookApi.GetSnapshot(scenarioId);
        }

        public void DeleteRun(Guid runId, Guid scenarioId)
        {
            _autoBookApi.DeleteRun(runId, scenarioId);
        }

        public GetAutoBookStorageInfoModel GetStorageInfo()
        {
            return _autoBookApi.GetStorageInfo();
        }
    }
}
