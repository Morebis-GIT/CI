using System.Linq;
using ImagineCommunications.Gameplan.Integration.Contracts.Interfaces.Clash;
using ImagineCommunications.Gameplan.Integration.Data.Context;
using ImagineCommunications.Gameplan.Integration.Data.Entities;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.Infrastructure.Interfaces;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using TechTalk.SpecFlow;
using Xunit;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.Steps.DataExchange
{
    [Binding, Scope(Tag = "Exchange")]
    public class DataExchangeSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly ITestDataImporter _testDataImporter;
        private bool _sendGroupTransactionInfo;
        private readonly IntelligenceDbContext _intelligenceDbContext;
        private IEventScenarioService _eventScenarioService;

        public DataExchangeSteps(ScenarioContext scenarioContext, ITestDataImporter testDataImporter, IntelligenceDbContext intelligenceDbContext)
        {
            _scenarioContext = scenarioContext;
            _testDataImporter = testDataImporter;
            _intelligenceDbContext = intelligenceDbContext;
            InsertPriorities();
        }

        [Given(@"GroupTransactionInfo for 1 event sent")]
        public void GivenGroupTransactionInfoSent()
        {
            _sendGroupTransactionInfo = true;
        }

        [Given(@"I have (.*) message to publish")]
        public void GivenIHaveMessageToPublish(string eventName, Table table)
        {
            _eventScenarioService = _eventScenarioService ?? _scenarioContext.ScenarioContainer.Resolve<IEventScenarioService>(eventName);
            if (_sendGroupTransactionInfo)
            {
                _eventScenarioService.PublishGroupTransaction();
            }
            _eventScenarioService.CreateEventModel(table);
        }

        [Given(@"I have (.*) message from file (.*) to publish")]
        public void GivenIHaveMessageFromFileToPublish(string eventName, string fileName)
        {
            _eventScenarioService = _eventScenarioService ?? _scenarioContext.ScenarioContainer.Resolve<IEventScenarioService>(eventName);
            if (_sendGroupTransactionInfo)
            {
                _eventScenarioService.PublishGroupTransaction();
            }
            _eventScenarioService.CreateEventModelFromFile(fileName);
        }

        [Given(@"The data from file (.*) exists in database")]
        public void GivenTheDataFromFileExists(string fileName)
        {
            _testDataImporter.FromResourceScript(fileName + ".json");
        }

        [Given(@"The (.*) data exists in GamePlan database")]
        public void GivenTheDataFromTableExists(string key, Table table)
        {
            _testDataImporter.ImportDataFromTable(key, table);
        }

        [When(@"I publish message to message broker")]
        public void WhenIPublishMessageToQueue()
        {
            _eventScenarioService.PublishMessage();
        }

        [Then(@"GamePlanIntelligence consumes message")]
        public void ThenConsumerConsumesMessage()
        {
            var result = _eventScenarioService.CheckConsumerSuccessfullyConsumed();
            Assert.True(result, "Message was not consumed");
        }

        [Then(@"the (.*) data in GamePlan database is updated as following")]
        public void ThenTheDataIsUpdated(string key, Table data)
        {
            IResultCheckerService resultChecker =
                _scenarioContext.ScenarioContainer.Resolve<IResultCheckerService>(key);
            var result = resultChecker.CompareTargetDataToDb(key, tableData: data,
                operationType: TestDataResultOperationType.Add);
            Assert.True(result, key + " not updated correctly");
        }

        [Then(@"the (.*) data in GamePlan database is updated as the data from the following file (.*)")]
        public void ThenTheDataIsUpdatedAsInTheFile(string key, string fileName)
        {
            IResultCheckerService resultChecker =
                _scenarioContext.ScenarioContainer.Resolve<IResultCheckerService>(key);
            var result = resultChecker.CompareTargetDataToDb(key, fileName: fileName,
                operationType: TestDataResultOperationType.Add);
            Assert.True(result, key + " not updated correctly");
        }

        [Then(@"the (.*) data is replaced as in the following file (.*)")]
        public void ThenTheDataIsReplacedAsInTheFile(string key, string fileName)
        {
            IResultCheckerService resultChecker =
                _scenarioContext.ScenarioContainer.Resolve<IResultCheckerService>(key);
            var result = resultChecker.CompareTargetDataToDb(key, fileName: fileName,
                operationType: TestDataResultOperationType.Replace);
            Assert.True(result, key + " not updated correctly");
        }

        [Then(@"the (.*) data in GamePlan database is updated as the data from the file (.*) added the following")]
        public void ThenTheDataIsUpdatedAsInTheFileAddTheFollowing(string key, string fileName, Table table)
        {
            IResultCheckerService resultChecker =
                _scenarioContext.ScenarioContainer.Resolve<IResultCheckerService>(key);
            var result = resultChecker.CompareTargetDataToDb(key, fileName: fileName, tableData: table,
                operationType: TestDataResultOperationType.Add);
            Assert.True(result, key + " not updated correctly");
        }

        [Then(@"the (.*) data in GamePlan database is updated as the data from the file (.*) removed the following")]
        public void ThenTheDataIsUpdatedAsInTheFileRemoveTheFollowing(string key, string fileName, Table table)
        {
            IResultCheckerService resultChecker =
                _scenarioContext.ScenarioContainer.Resolve<IResultCheckerService>(key);
            var result = resultChecker.CompareTargetDataToDb(key, fileName: fileName, tableData: table,
                operationType: TestDataResultOperationType.Remove);
            Assert.True(result, key + " not updated correctly");
        }

        [Then(@"the (.*) data in GamePlan database is updated as the data from the file (.*) replaced the following")]
        public void ThenTheDataIsUpdatedAsInTheFileReplaceTheFollowing(string key, string fileName, Table table)
        {
            IResultCheckerService resultChecker =
                _scenarioContext.ScenarioContainer.Resolve<IResultCheckerService>(key);
            var result = resultChecker.CompareTargetDataToDb(key, fileName: fileName, tableData: table,
                operationType: TestDataResultOperationType.Replace);
            Assert.True(result, key + " not updated correctly");
        }

        [Then(@"GamePlanIntelligence throws ContractValidation exception for following properties")]
        public void ThenConsumerThrowsContractValidationException(Table errorFields)
        {
            var errorWasThrown = _eventScenarioService.CheckContractValidationFields(errorFields);
            Assert.True(errorWasThrown, $"Contract Validation fields mismatch");
        }

        [Then(@"GamePlanIntelligence throws exception with the code: (.*)")]
        public void ThenConsumerThrowsSpecificExceptionWithErrorCode(string errorCode)
        {
            var errorWasThrown = _eventScenarioService.CheckDataSyncErrorCode(errorCode);
            Assert.True(errorWasThrown, $"Consumer did not throw exception with code {errorCode}");
        }

        private void InsertPriorities()
        {
            if (!_intelligenceDbContext.MessageTypes.Any())
            {
                var data = new Gameplan.Integration.Data.Entities.MessageEntityType() { Name = "Entity" };

                _ = _intelligenceDbContext.MessageEntityTypes.Add(data);

                _ = _intelligenceDbContext.MessageTypes.Add(new MessageType() { Id = nameof(IBulkClashCreatedOrUpdated), Priority = 1, MessageEntityType = data });

                _ = _intelligenceDbContext.SaveChanges();
            }
        }
    }
}
