using System;
using TechTalk.SpecFlow;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.Interfaces
{
    public interface IEventScenarioService
    {
        Guid? GroupTransactionId { get; set; }
        void CreateEventModel(Table table);
        void CreateEventModelFromFile(string fileName);
        void PublishMessage();
        bool CheckConsumerSuccessfullyConsumed();
        bool CheckContractValidationFields(Table fields);
        bool CheckDataSyncErrorCode(string errorCode);
        void PublishGroupTransaction();
    }
}
