using System.Linq;
using ImagineCommunications.GamePlan.Domain.DayParts.Objects;
using ImagineCommunications.GamePlan.Domain.DayParts.Repositories;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using TechTalk.SpecFlow;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices
{
    public class StandardDayPartResultChecker : ResultCheckerService<StandardDayPart>
    {
        private readonly IStandardDayPartRepository _standardDayPartRepository;

        public StandardDayPartResultChecker(ITestDataImporter testDataImporter, IStandardDayPartRepository standardDayPartRepository) : base(testDataImporter)
        {
            _standardDayPartRepository = standardDayPartRepository;
        }

        public override bool CompareTargetDataToDb(string key, string fileName = null, Table tableData = null,
            TestDataResultOperationType operationType = default)
        {
            var featureTestData = GenerateDataFromTable(tableData);
            var fileTestData = GenerateDataFromFile(fileName, key);
            var dayParts = _standardDayPartRepository.GetAll().ToList();

            switch (operationType)
            {
                case TestDataResultOperationType.Add:
                {
                    featureTestData.AddRange(fileTestData);
                    return dayParts.Count == featureTestData.Count && featureTestData.All(entity => dayParts.Count(c => AreSameDayParts(c, entity)) == 1);
                }
                case TestDataResultOperationType.Remove:
                case TestDataResultOperationType.Replace:
                    return true;
                default:
                    return false;
            }
        }

        private static bool AreSameDayParts(StandardDayPart source, StandardDayPart target) =>
            source.SalesArea == target.SalesArea &&
            source.Name == target.Name &&
            source.DayPartId == target.DayPartId &&
            source.Order == target.Order &&
            source.Timeslices.Count == target.Timeslices.Count;
    }
}
