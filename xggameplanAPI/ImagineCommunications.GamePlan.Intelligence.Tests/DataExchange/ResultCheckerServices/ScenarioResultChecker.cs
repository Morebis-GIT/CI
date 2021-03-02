using System.Linq;
using ImagineCommunications.GamePlan.Domain.Scenarios;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using TechTalk.SpecFlow;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices
{
    public class ScenarioResultChecker : ResultCheckerService<Scenario>
    {
        private readonly IScenarioRepository _scenarioRepository;

        public ScenarioResultChecker(ITestDataImporter dataImporter, IScenarioRepository scenarioRepository) : base(dataImporter) =>
            _scenarioRepository = scenarioRepository;

        public override bool CompareTargetDataToDb(string key, string fileName = null, Table tableData = null, TestDataResultOperationType operationType = default)
        {
            var featureTestData = GenerateDataFromTable(tableData);
            var fileTestData = GenerateDataFromFile(fileName, key);
            var scenarios = _scenarioRepository.GetAll().ToList();

            switch (operationType)
            {
                case TestDataResultOperationType.Add:
                {
                    featureTestData.AddRange(fileTestData);
                    return scenarios.Count == featureTestData.Count && featureTestData.All(entity => scenarios.Count(c => c.Id == entity.Id) == 1);
                }
                case TestDataResultOperationType.Remove:
                case TestDataResultOperationType.Replace:
                    return true;
                default:
                    return false;
            }
        }
    }
}
