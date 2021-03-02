using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Universes;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using TechTalk.SpecFlow;
using UniverseDomainObject = ImagineCommunications.GamePlan.Domain.Shared.Universes.Universe;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Universe
{
    public class UniverseResultChecker : ResultCheckerService<UniverseDomainObject>
    {
        private readonly IUniverseRepository _universeRepository;

        public UniverseResultChecker(ITestDataImporter dataImporter, IUniverseRepository universeRepository) : base(dataImporter) => _universeRepository = universeRepository;

        public override bool CompareTargetDataToDb(string key, string fileName = null, Table tableData = null, TestDataResultOperationType operationType = default)
        {
            var featureTestData = GenerateDataFromTable(tableData);
            var fileTestData = GenerateDataFromFile(fileName, key);
            var universes = _universeRepository.GetAll().ToList();

            switch (operationType)
            {
                case TestDataResultOperationType.Add:
                {
                    featureTestData.AddRange(fileTestData);
                    return universes.Count == featureTestData.Count && featureTestData.All(entity => universes.Count(c => AreSameUniverses(c, entity)) == 1);
                }
                case TestDataResultOperationType.Remove:
                    return universes.Count == fileTestData.Count - featureTestData.Count && universes.All(x => x.Id != featureTestData.FirstOrDefault()?.Id);
                case TestDataResultOperationType.Replace:
                {
                    var existingUniverseId = featureTestData.FirstOrDefault()?.Id;
                    if (existingUniverseId is null)
                    {
                        return false;
                    }
                    
                    var existingUniverse = _universeRepository.Find(existingUniverseId.Value);
                    var toCheck = fileTestData.FirstOrDefault(x => x.Id == existingUniverseId);
                    
                    return existingUniverse.Id != toCheck?.Id;
                }
                default:
                    return false;
            }
        }

        private static bool AreSameUniverses(UniverseDomainObject universe1, UniverseDomainObject universe2) =>
            universe1.SalesArea == universe2.SalesArea && universe1.Demographic == universe2.Demographic &&
            universe1.StartDate == universe2.StartDate && universe1.EndDate == universe2.EndDate;
    }
}
