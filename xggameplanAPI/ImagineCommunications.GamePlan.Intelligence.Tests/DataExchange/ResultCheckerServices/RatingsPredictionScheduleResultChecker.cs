using System.Linq;
using ImagineCommunications.GamePlan.Domain.RatingSchedules;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using TechTalk.SpecFlow;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices
{
    public class RatingsPredictionScheduleResultChecker : ResultCheckerService<RatingsPredictionSchedule>
    {
        private readonly IRatingsScheduleRepository _ratingsScheduleRepository;

        public RatingsPredictionScheduleResultChecker(ITestDataImporter dataImporter, IRatingsScheduleRepository ratingsScheduleRepository) : base(dataImporter) =>
            _ratingsScheduleRepository = ratingsScheduleRepository;

        public override bool CompareTargetDataToDb(string key, string fileName = null, Table tableData = null, TestDataResultOperationType operationType = default)
        {
            var featureTestData = GenerateDataFromTable(tableData);
            var fileTestData = GenerateDataFromFile(fileName, key);
            var schedules = _ratingsScheduleRepository.GetAll().ToList();

            switch (operationType)
            {
                case TestDataResultOperationType.Add:
                {
                    featureTestData.AddRange(fileTestData);
                    if (schedules.Count != featureTestData.Count)
                    {
                        return false;
                    }
                    
                    return featureTestData.All(entity => schedules.Count(c =>
                                                   c.SalesArea == entity.SalesArea &&
                                                   c.ScheduleDay == entity.ScheduleDay &&
                                                   c.Ratings.Count == entity.Ratings.Count) == 1);
                }
                default:
                    return false;
            }
        }
    }
}
