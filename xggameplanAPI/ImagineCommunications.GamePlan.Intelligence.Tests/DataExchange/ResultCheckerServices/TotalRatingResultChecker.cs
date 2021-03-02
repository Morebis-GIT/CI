using System.Linq;
using ImagineCommunications.GamePlan.Domain.TotalRatings;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using TechTalk.SpecFlow;
using TotalRatingDbObject = ImagineCommunications.GamePlan.Domain.TotalRatings.TotalRating;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices
{
    public class TotalRatingResultChecker : ResultCheckerService<TotalRatingDbObject>
    {
        private readonly ITotalRatingRepository _totalRatingRepository;

        public TotalRatingResultChecker(ITestDataImporter testDataImporter, ITotalRatingRepository totalRatingRepository) : base(testDataImporter)
        {
            _totalRatingRepository = totalRatingRepository;
        }

        public override bool CompareTargetDataToDb(string key, string fileName = null, Table tableData = null,
            TestDataResultOperationType operationType = default)
        {
            var featureTestData = GenerateDataFromTable(tableData);
            var fileTestData = GenerateDataFromFile(fileName, key);
            var totalRatings = _totalRatingRepository.GetAll().ToList();

            switch (operationType)
            {
                case TestDataResultOperationType.Add:
                {
                    featureTestData.AddRange(fileTestData);
                    return totalRatings.Count == featureTestData.Count && featureTestData.All(entity => totalRatings.Count(c => AreSameTotalRatings(c, entity)) == 1);
                }
                case TestDataResultOperationType.Remove:
                case TestDataResultOperationType.Replace:
                    return true;
                default:
                    return false;
            }
        }

        private static bool AreSameTotalRatings(TotalRatingDbObject totalRating1, TotalRatingDbObject totalRating2) =>
            totalRating1.Date == totalRating2.Date &&
            totalRating1.SalesArea == totalRating2.SalesArea &&
            totalRating1.Daypart == totalRating2.Daypart &&
            totalRating1.DaypartGroup == totalRating2.DaypartGroup &&
            totalRating1.Demograph == totalRating2.Demograph &&
            totalRating1.TotalRatings == totalRating2.TotalRatings;
    }
}
