using System;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using TechTalk.SpecFlow;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Programme
{
    public class ProgrammeScheduleResultChecker : ResultCheckerService<Schedule>
    {
        private readonly IScheduleRepository _scheduleRepository;

        public ProgrammeScheduleResultChecker(ITestDataImporter dataImporter, IScheduleRepository scheduleRepository) : base(dataImporter) =>
            _scheduleRepository = scheduleRepository;

        public override bool CompareTargetDataToDb(string key, string fileName = null, Table tableData = null, TestDataResultOperationType operationType = default)
        {
            var fileTestData = GenerateDataFromFile(fileName, key);
            var schedules = _scheduleRepository.GetAll().ToList();

            switch (operationType)
            {
                case TestDataResultOperationType.Add:
                {
                    return schedules.Count == fileTestData.Count && fileTestData.All(entity =>
                               schedules.Count(c =>
                                   c.SalesArea.Equals(entity.SalesArea, StringComparison.CurrentCultureIgnoreCase) &&
                                   c.Date == entity.Date) == 1);
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
