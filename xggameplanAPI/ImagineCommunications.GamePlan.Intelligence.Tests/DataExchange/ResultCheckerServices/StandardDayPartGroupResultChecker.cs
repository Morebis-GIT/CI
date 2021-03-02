using System.Linq;
using ImagineCommunications.GamePlan.Domain.DayParts.Objects;
using ImagineCommunications.GamePlan.Domain.DayParts.Repositories;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using TechTalk.SpecFlow;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices
{
    public class StandardDayPartGroupResultChecker : ResultCheckerService<StandardDayPartGroup>
    {
        private readonly IStandardDayPartGroupRepository _standardDayPartGroupRepository;

        public StandardDayPartGroupResultChecker(ITestDataImporter testDataImporter, IStandardDayPartGroupRepository standardDayPartGroupRepository) : base(testDataImporter)
        {
            _standardDayPartGroupRepository = standardDayPartGroupRepository;
        }

        public override bool CompareTargetDataToDb(string key, string fileName = null, Table tableData = null,
            TestDataResultOperationType operationType = default)
        {
            var featureTestData = GenerateDataFromTable(tableData);
            var fileTestData = GenerateDataFromFile(fileName, key);
            var dayPartGroups = _standardDayPartGroupRepository.GetAll().ToList();

            switch (operationType)
            {
                case TestDataResultOperationType.Add:
                {
                    featureTestData.AddRange(fileTestData);
                    return dayPartGroups.Count == featureTestData.Count && featureTestData.All(entity => dayPartGroups.Count(c => AreSameDayPartGroups(c, entity)) == 1);
                }
                case TestDataResultOperationType.Remove:
                case TestDataResultOperationType.Replace:
                    return true;
                default:
                    return false;
            }
        }

        private static bool AreSameDayPartGroups(StandardDayPartGroup source, StandardDayPartGroup target) =>
            source.SalesArea == target.SalesArea &&
            source.Demographic == target.Demographic &&
            source.GroupId == target.GroupId &&
            source.Optimizer == target.Optimizer &&
            source.RatingReplacement == target.RatingReplacement &&
            source.Policy == target.Policy &&
            source.Splits.Count == target.Splits.Count;
    }
}
