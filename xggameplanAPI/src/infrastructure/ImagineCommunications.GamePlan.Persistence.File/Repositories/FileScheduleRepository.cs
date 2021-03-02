using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using xgcore.auditevents.Repository.File;

namespace ImagineCommunications.GamePlan.Persistence.File.Repositories
{
    public class FileScheduleRepository : FileRepositoryBase, IScheduleRepository
    {
        public FileScheduleRepository(string folder) : base(folder, "schedule")
        {
        }

        public Schedule Get(int id)
        {
            return GetItemByID<Schedule>(_folder, _type, id.ToString());
        }

        public IEnumerable<Schedule> GetAll()
        {
            return GetAllByType<Schedule>(_folder, _type);
        }

        public void Delete(int id)
        {
            DeleteItem(_folder, _type, id.ToString());
        }

        public int CountAll => GetAllByType<Schedule>(_folder, _type).Count;

        /// <summary>
        /// Add new entry in schedule
        /// </summary>
        /// <param name="item"></param>
        public void Add(Schedule item)
        {
            List<Schedule> items = new List<Schedule>() { item };
            InsertItems(_folder, _type, items, items.ConvertAll(i => i.Id.ToString()));
        }

        /// <summary>
        /// Updates schedule item
        /// </summary>
        /// <param name="item"></param>
        public void Update(Schedule item)
        {
            UpdateOrInsertItem(_folder, _type, item, item.Id.ToString());
        }

        public int GetScheduleBreaksCount(string salesAreaName, DateTime scheduleDate)
        {
            return GetSchedule(salesAreaName, scheduleDate)?.Breaks?.Count ?? 0;
        }

        /// <summary>
        /// GetSchedule by list sales area names and schedule dates
        /// </summary>
        /// <param name="salesAreaNames"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public List<Schedule> GetSchedule(List<string> salesAreaNames, DateTime fromDate, DateTime toDate)
        {
            return GetAllByType<Schedule>(_folder, _type, s => s.Date >= fromDate.Date
                                                       && s.Date < toDate.Date.AddDays(1)
                                                       && salesAreaNames.Contains(s.SalesArea));
        }

        public List<BreakSimple> GetScheduleSimpleBreaks(List<string> salesAreaNames, DateTime fromDate, DateTime toDate)
        {
            return GetAllByType<Schedule>(_folder, _type, s => s.Date >= fromDate.Date
                && s.Date < toDate.Date.AddDays(1)
                && salesAreaNames.Contains(s.SalesArea))
                .Where(x => x.Breaks != null)
                .SelectMany(s => s.Breaks.Select(b =>
                    new BreakSimple
                    {
                        SalesAreaName = b.SalesArea,
                        ScheduleDate = b.ScheduledDate.Date,
                        CustomId = b.CustomId,
                        ExternalBreakRef = b.ExternalBreakRef
                    }))
                .ToList();
        }

        public List<BreakWithProgramme> GetBreakModels(List<string> salesAreaNames, DateTime fromDate, DateTime toDate, string excludeBreakType)
        {
            var schedules = GetAllByType<Schedule>(_folder, _type, s => s.Date >= fromDate.Date
                                                           && s.Date < toDate.Date
                                                               .AddDays(1)
                                                           && salesAreaNames.Contains(s.SalesArea));
            if (schedules.Count == 0)
            {
                return null;
            }

            var index = (from schedule in schedules.Where(s => s.Breaks?.Count > 0)
                         let programmes = schedule.Programmes?.OrderBy(d => d.StartDateTime)

                         // Get first & last prog on the schedule
                         let programmeFirst = programmes == null ? null : programmes.OrderBy(p => p.StartDateTime).FirstOrDefault()
                         let programmeLast = programmes == null ? null : programmes.OrderBy(p => p.StartDateTime.Add(p.Duration.ToTimeSpan())).LastOrDefault()

                         // Get breaks for all schedule progs, prog spanning
                         // calendar days will have breaks in multiple Schedule docs
                         let breaks = schedules.Where(s => s.SalesArea == schedule.SalesArea && s.Breaks != null && s.Breaks.Count > 0 &&
                                                s.Date >= schedule.Date && s.Date <= schedule.Date.AddDays(1))
                                                .SelectMany(s => s.Breaks)
                                                .Where(b => programmeFirst != null && programmeLast != null &&
                                                b.ScheduledDate >= fromDate && b.ScheduledDate <= toDate &&
                                                b.ScheduledDate >= programmeFirst.StartDateTime && b.ScheduledDate.Add(b.Duration.ToTimeSpan()) <= programmeLast.StartDateTime.Add(programmeLast.Duration.ToTimeSpan()))

                         from Break brk in breaks.Where(b => b.BreakType != excludeBreakType)
                         let pgm = programmes != null && programmes.Any()
                    ? programmes.FirstOrDefault(p => p.StartDateTime <= brk.ScheduledDate &&
                                                     p.StartDateTime.AddTicks(p.Duration.BclCompatibleTicks) >=
                                                     brk.ScheduledDate)
                    : null
                         select new BreakWithProgramme
                         {
                             Break = brk,
                             //For now default to SPORTS programme category if null
                             ProgrammeCategories = (pgm?.ProgrammeCategories.Count ?? 0) > 0 ? (pgm.ProgrammeCategories) : new List<string>() { "SPORTS" },
                             ProgrammeExternalreference = pgm?.ExternalReference,
                             PrgtNo = pgm?.PrgtNo ?? 0
                         }).ToList();

            return index;
        }

        public List<Tuple<Break, Programme>> GetBreakWithProgramme(
            List<string> salesAreaNames,
            DateTime fromDate,
            DateTime toDate)
        {
            var schedules = GetAllByType<Schedule>(_folder, _type, s => s.Date >= fromDate.Date
                                                       && s.Date < toDate.Date.AddDays(1)
                                                       && salesAreaNames.Contains(s.SalesArea));

            if (schedules.Count == 0)
            {
                return null;
            }

            var index = (from schedule in schedules.Where(s => s.Programmes != null && s.Programmes.Any())
                         let programmes = schedule.Programmes?.OrderBy(d => d.StartDateTime)
                                                                      .Where(p => p.StartDateTime >= fromDate && p.StartDateTime <= toDate)

                         // Get first & last prog on the schedule
                         let programmeFirst = programmes == null ? null : programmes.OrderBy(p => p.StartDateTime).FirstOrDefault()
                         let programmeLast = programmes == null ? null : programmes.OrderBy(p => p.StartDateTime.Add(p.Duration.ToTimeSpan())).LastOrDefault()

                         // Get breaks for all schedule progs, prog spanning
                         // calendar days will have breaks in multiple Schedule docs
                         let breaks = schedules.Where(s => s.SalesArea == schedule.SalesArea && s.Breaks != null && s.Breaks.Any() &&
                                         s.Date >= schedule.Date && s.Date <= schedule.Date.AddDays(1))
                                         .SelectMany(s => s.Breaks)
                                         .Where(b => programmeFirst != null && programmeLast != null &&
                                         b.ScheduledDate >= fromDate && b.ScheduledDate <= toDate &&
                                         b.ScheduledDate >= programmeFirst.StartDateTime && b.ScheduledDate.Add(b.Duration.ToTimeSpan()) <= programmeLast.StartDateTime.Add(programmeLast.Duration.ToTimeSpan()))

                         from pgm in programmes
                         let brks = breaks.Where(b => pgm.StartDateTime <= b.ScheduledDate &&
                                                                                            pgm.StartDateTime.AddTicks(pgm.Duration
                                                                                                .BclCompatibleTicks) >=
                                                                                            b.ScheduledDate)        //.EmptyIfNull()
                         from brk in brks.DefaultIfEmpty()

                         select Tuple.Create(brk, pgm)).ToList();
            return index;
        }

        /// <summary>
        /// GetSchedule by channel id and schedule date
        /// </summary>
        /// <param name="salesAreaName"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public Schedule GetSchedule(string salesAreaName, DateTime date)
        {
            return GetAllByType<Schedule>(_folder, _type, s => s.SalesArea == salesAreaName && s.Date == date).FirstOrDefault();
        }

        /// <summary>
        /// GetBreaks by list sales area names and schedule dates
        /// </summary>
        /// <param name="salesAreaNames"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <returns></returns>
        public List<Break> GetBreaks(List<string> salesAreaNames, DateTime fromDate, DateTime toDate)
        {
            var schedules = GetAllByType<Schedule>(_folder, _type, s => s.Date >= fromDate.Date
                                                        && s.Date < toDate.Date.AddDays(1)
                                                        && salesAreaNames.Contains(s.SalesArea));

            return schedules.Count > 0
                ? schedules
                    .Where(b => b.Breaks != null)
                    .SelectMany(s => s.Breaks)
                    .Where(x => x.ScheduledDate >= fromDate && x.ScheduledDate <= toDate)
                    .ToList()
                : null;
        }

        public List<Programme> GetProgrammes(List<string> salesAreaNames, DateTime fromDate, DateTime toDate)
        {
            var schedules = GetAllByType<Schedule>(_folder, _type, s => s.Date >= fromDate.Date
                                                              && s.Date < toDate.Date.AddDays(1)
                                                              && salesAreaNames.Contains(s.SalesArea));
            return schedules.Count > 0 ? (schedules.Where(b => b.Programmes != null).SelectMany(s => s.Programmes).ToList()) : null;
        }

        public List<Schedule> FindByBreakIds(IEnumerable<Guid> breakIds)
        {
            return GetAllByType<Schedule>(_folder, _type, s => s.Breaks.Any(b => breakIds.Contains(b.Id)));
        }

        public void Truncate()
        {
            DeleteAllItems(_folder, _type);
        }

        public Task TruncateAsync()
        {
            Truncate();
            return Task.CompletedTask;
        }

        public void SaveChanges()
        {
        }

        public Task SaveChangesAsync() => Task.CompletedTask;

        public void Dispose()
        {
        }

        public int Count(Expression<Func<Schedule, bool>> query)
        {
            return GetAllByType(_folder, _type, query).Count;
        }

        public (int breaksCount, int programmesCount) CountBreaksAndProgrammes(DateTime dateFrom, DateTime dateTo)
        {
            var schedules = GetAllByType<Schedule>(_folder, _type, s => s.Date >= dateFrom
                                                              && s.Date <= dateTo);
            var breaksCount = schedules.Where(x => x.Programmes != null).SelectMany(x => x.Breaks).Count();
            var programmesCount = schedules.Where(x => x.Programmes != null).SelectMany(x => x.Programmes).Count();
            return (breaksCount, programmesCount);
        }
    }
}
