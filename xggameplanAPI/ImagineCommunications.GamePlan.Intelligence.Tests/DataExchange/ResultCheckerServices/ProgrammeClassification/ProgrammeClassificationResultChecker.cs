using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.ProgrammeClassifications;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using TechTalk.SpecFlow;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.ProgrammeClassification
{
    public class ProgrammeClassificationResultChecker : ResultCheckerService<Domain.Shared.ProgrammeClassifications.ProgrammeClassification>
    {
        private readonly IProgrammeClassificationRepository _programmeClassificationRepository;

        public ProgrammeClassificationResultChecker(ITestDataImporter testDataImporter, IProgrammeClassificationRepository programmeClassificationRepository) : base(testDataImporter)
        {
            _programmeClassificationRepository = programmeClassificationRepository;
        }

        public override bool CompareTargetDataToDb(string key, string fileName = null, Table tableData = null, TestDataResultOperationType operationType = default)
        {
            var featureTestData = GenerateDataFromTable(tableData);
            var fileTestData = GenerateDataFromFile(fileName, key);
            var programmeClassifications = _programmeClassificationRepository.GetAll().ToList();

            switch (operationType)
            {
                case TestDataResultOperationType.Add:
                {
                    featureTestData.AddRange(fileTestData);
                    return programmeClassifications.Count == featureTestData.Count && featureTestData.All(entity => programmeClassifications.Count(c => c.Uid == entity.Uid) == 1);
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
