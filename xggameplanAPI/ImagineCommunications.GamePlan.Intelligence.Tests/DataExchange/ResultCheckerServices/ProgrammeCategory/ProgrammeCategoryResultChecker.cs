using System.Linq;
using ImagineCommunications.GamePlan.Domain.ProgrammeCategory;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using TechTalk.SpecFlow;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.ProgrammeCategory
{
    public class ProgrammeCategoryResultChecker : ResultCheckerService<ProgrammeCategoryHierarchy>
    {
        private readonly IProgrammeCategoryHierarchyRepository _programmeCategoryRepository;

        public ProgrammeCategoryResultChecker(ITestDataImporter testDataImporter, IProgrammeCategoryHierarchyRepository programmeCategoryRepository) : base(testDataImporter)
        {
            _programmeCategoryRepository = programmeCategoryRepository;
        }

        public override bool CompareTargetDataToDb(string key, string fileName = null, Table tableData = null,
            TestDataResultOperationType operationType = default)
        {
            var featureTestData = GenerateDataFromTable(tableData);
            var fileTestData = GenerateDataFromFile(fileName, key);
            var programmeCategories = _programmeCategoryRepository.GetAll().ToList();

            switch (operationType)
            {
                case TestDataResultOperationType.Add:
                {
                    featureTestData.AddRange(fileTestData);
                    return programmeCategories.Count == featureTestData.Count && featureTestData.All(entity => programmeCategories.Count(c => c.Name == entity.Name) == 1);
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
