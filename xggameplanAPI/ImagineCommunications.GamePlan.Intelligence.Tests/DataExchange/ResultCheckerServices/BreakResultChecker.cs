using System.Linq;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Common;
using ImagineCommunications.GamePlan.Intelligence.Tests.Infrastructure.Interfaces;
using TechTalk.SpecFlow;

namespace ImagineCommunications.GamePlan.Intelligence.Tests.DataExchange.ResultCheckerServices
{
    public class BreakResultChecker : ResultCheckerService<Break>
    {
        private readonly IBreakRepository _breakRepository;
        private readonly ISpotRepository _spotRepository;
        private readonly IScheduleRepository _scheduleRepository;
        
        public BreakResultChecker(
            ITestDataImporter dataImporter,
            IBreakRepository breakRepository,
            IScheduleRepository scheduleRepository,
            ISpotRepository spotRepository) : base(dataImporter)
        {
            _breakRepository = breakRepository;
            _scheduleRepository = scheduleRepository;
            _spotRepository = spotRepository;
        }
        
        public override bool CompareTargetDataToDb(string key, string fileName = null, Table tableData = null,
            TestDataResultOperationType operationType = default)
        {
            var featureTestData = GenerateDataFromTable(tableData);
            var fileTestData = GenerateDataFromFile(fileName, key);
            var allBreaks = _breakRepository.GetAll().ToList();
            
            switch (operationType)
            {
                case TestDataResultOperationType.Add:
                {
                    featureTestData.AddRange(fileTestData);
                    if (allBreaks.Count != featureTestData.Count)
                    {
                        return false;
                    }
                    
                    foreach (var entity in featureTestData)
                    {
                        if (allBreaks.Count(c => c.ExternalBreakRef == entity.ExternalBreakRef) != 1)
                        {
                            return false;
                        }
                        
                        var storedBreak = allBreaks.FirstOrDefault(c => c.ExternalBreakRef == entity.ExternalBreakRef);
                        if (!CompareBreak(entity, storedBreak))
                        {
                            return false;
                        }
                    }

                    var resultSchedules = TestDataImporter.GetDataFromFile<Schedule>(fileName, "Schedules").ToList();
                    var allSchedules = _scheduleRepository.GetAll().ToList();
                    if (allSchedules.Count != resultSchedules.Count)
                    {
                        return false;
                    }
                    
                    foreach (var schedule in resultSchedules)
                    {
                        var storedSchedule = allSchedules.FirstOrDefault(c => c.SalesArea == schedule.SalesArea && c.Date == schedule.Date);
                        var storedRefs = storedSchedule.Breaks.Select(b => b.ExternalBreakRef).OrderBy(b => b);
                        var targetRefs = schedule.Breaks.Select(b => b.ExternalBreakRef).OrderBy(b => b);
                        if (!storedRefs.SequenceEqual(targetRefs))
                        {
                            return false;
                        }
                    }
                    
                    var resultSpots = TestDataImporter.GetDataFromFile<Schedule>(fileName, "Spots").ToList();
                    var allSpots = _spotRepository.GetAll();
                    
                    return allSpots.Count() == resultSpots.Count;
                }
                default:
                    return false;
            }
        }
        
        public bool CompareBreak(Break source, Break target) =>
            source.ScheduledDate.Date == target.ScheduledDate.Date &&
            source.Description == target.Description &&
            source.BreakType == target.BreakType &&
            source.Duration == target.Duration &&
            source.ExternalBreakRef == target.ExternalBreakRef &&
            source.PositionInProg == target.PositionInProg &&
            source.ExternalProgRef == target.ExternalProgRef;
    }
}
