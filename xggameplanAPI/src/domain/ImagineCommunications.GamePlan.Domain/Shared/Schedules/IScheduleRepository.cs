using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;

namespace ImagineCommunications.GamePlan.Domain.Shared.Schedules
{
    public interface IScheduleRepository
    {
        Schedule Get(int id);

        void Delete(int id);

        IEnumerable<Schedule> GetAll();

        int CountAll { get; }

        int Count(Expression<Func<Schedule, bool>> query);

        (int breaksCount, int programmesCount) CountBreaksAndProgrammes(DateTime dateFrom, DateTime dateTo);

        void Add(Schedule item);

        void Update(Schedule item);

        Schedule GetSchedule(string salesareaname, DateTime scheduledate);

        int GetScheduleBreaksCount(string salesAreaName, DateTime scheduleDate);

        List<Schedule> GetSchedule(List<string> salesareanames, DateTime fromdate, DateTime todate);

        List<BreakSimple> GetScheduleSimpleBreaks(List<string> salesAreaNames, DateTime fromDate, DateTime toDate);

        List<Break> GetBreaks(List<string> salesareanames, DateTime fromdate, DateTime todate);

        List<Programme> GetProgrammes(List<string> salesareanames, DateTime fromdate, DateTime todate);

        List<Tuple<Break, Programme>> GetBreakWithProgramme(List<string> salesAreaNames, DateTime fromDate, DateTime toDate);

        List<Schedule> FindByBreakIds(IEnumerable<Guid> breakIds);

        void Truncate();

        List<BreakWithProgramme> GetBreakModels(List<string> salesAreaNames,
            DateTime fromDate, DateTime toDate, string excludeBreakType);

        void SaveChanges();

        Task SaveChangesAsync();

        Task TruncateAsync();
    }
}
