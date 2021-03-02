using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using TechTalk.SpecFlow;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices
{
    public class DemographicResultChecker : ResultCheckerService<Demographic>
    {
        private readonly IDemographicRepository _demographicsRepository;

        public DemographicResultChecker(ITestDataImporter dataImporter, IDemographicRepository demographicsRepository) : base(dataImporter)
        {
            _demographicsRepository = demographicsRepository;
        }

        public override bool CompareTargetDataToDb(string key, string fileName = null, Table tableData = null, TestDataResultOperationType operationType = 0)
        {
            var featureTestData = GenerateDataFromTable(tableData);
            var fileTestData = GenerateDataFromFile(fileName, key);
            var allDemos = _demographicsRepository.GetAll().ToList();
            
            switch (operationType)
            {
                case TestDataResultOperationType.Add:
                    {
                        featureTestData.AddRange(fileTestData);
                        if (allDemos.Count != featureTestData.Count)
                        {
                            return false;
                        }

                        foreach (var entity in featureTestData)
                        {
                            if (allDemos.Count(c => c.ExternalRef == entity.ExternalRef) != 1)
                            {
                                return false;
                            }
                            
                            var storedDemo = allDemos.FirstOrDefault(d => d.ExternalRef == entity.ExternalRef);
                            if (!CompareDemographic(entity, storedDemo))
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                default:
                    return false;
            }
        }

        private static bool CompareDemographic(Demographic source, Demographic target) =>
            source.Name == target.Name &&
            source.ExternalRef == target.ExternalRef &&
            source.Gameplan == target.Gameplan &&
            source.ShortName == target.ShortName &&
            source.DisplayOrder == target.DisplayOrder &&
            source.Id == target.Id;
    }
}
