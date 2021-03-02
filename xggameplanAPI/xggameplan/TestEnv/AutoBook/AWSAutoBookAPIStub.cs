using System;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage;
using xggameplan.AutoBooks.Abstractions;
using xggameplan.Model;
using AutoBookDomainObject = ImagineCommunications.GamePlan.Domain.AutoBookApi.Storage.Objects.AutoBook;

namespace xggameplan.TestEnv.AutoBook
{
    public class AWSAutoBookAPIStub : IAutoBookAPI
    {
        private readonly AutoBookDomainObject _autoBook;
        private readonly IAutoBooksTestHandler _autoBooksTestHandler;

        public AWSAutoBookAPIStub(AutoBookDomainObject autoBook, IAutoBooksTestHandler autoBooksTestHandler)
        {
            _autoBook = autoBook;
            _autoBooksTestHandler = autoBooksTestHandler;
        }

        public void DeleteRun(Guid runId, Guid scenarioId) { }

        public GetAutoBookAuditTrailModel GetAuditTrail(Guid scenarioId) =>
            new GetAutoBookAuditTrailModel()
            {
                Message = $"Stubbed test data: Simple Audit Trail Message for {scenarioId}"
            };

        public GetAutoBookLogsModel GetLogs(Guid scenarioId) =>
            new GetAutoBookLogsModel()
            {
                Message = $"Stubbed test data: Simple Logs Message for {scenarioId}"
            };

        public GetAutoBookSnapshotModel GetSnapshot(Guid scenarioId) =>
            new GetAutoBookSnapshotModel()
            {
                Data = Array.Empty<byte>()
            };

        public AutoBookStatuses GetStatus() => _autoBooksTestHandler.GetStatus(_autoBook.Id);

        public GetAutoBookStorageInfoModel GetStorageInfo() =>
            new GetAutoBookStorageInfoModel()
            {
                Available = "50GB",
                Total = "50GB",
                Used = "0GB"
            };

        public GetAutoBookVersionModel GetVersion() =>
            new GetAutoBookVersionModel()
            {
                Version = _autoBooksTestHandler.GetVersion(_autoBook.Id)
            };

        public GetAutoBookStatusModel StartAutoBookRun(Guid runId, Guid scenarioId)
        {
            _autoBooksTestHandler.ChangeStatus(_autoBook.Id, AutoBookStatuses.In_Progress);
            return new GetAutoBookStatusModel()
            {
                Status = GetStatus()
            };
        }
    }
}
