using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Extensions;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Helpers;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;
using xggameplan.core.Comparers;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Repositories
{
    public class RavenScheduleRepository : IScheduleRepository
    {
        private readonly IDocumentSession _session;
        private readonly IAsyncDocumentSession _sessionAsync;

        public RavenScheduleRepository(IDocumentSession session, IAsyncDocumentSession sessionAsync)
        {
            _session = session;
            _sessionAsync = sessionAsync;
        }

        public IEnumerable<Schedule> GetAll()
        {
            return _session.Query<Schedule>().Take(int.MaxValue).ToList();
        }

        public int CountAll
        {
            get
            {
                lock (_session)
                {
                    return _session.CountAll<Schedule>();
                }
            }
        }

        public int Count(Expression<Func<Schedule, bool>> query)
        {
            lock (_session)
            {
                return _session.Query<Schedule>().Where(query).Count();
            }
        }

        public (int breaksCount, int programmesCount) CountBreaksAndProgrammes(DateTime dateFrom, DateTime dateTo)
        {
            lock (_session)
            {
                var results = _session.Query<ScheduleDocumentsCount.Result, ScheduleDocumentsCount>()
                                      .Where(x => x.Date >= dateFrom && x.Date <= dateTo)
                                      .ToList();

                var breaksCount = results.Sum(x => x.BreaksCount);
                var programmesCount = results.Sum(x => x.ProgrammesCount);

                return (breaksCount, programmesCount);
            }
        }

        /// <summary>
        /// Add new entry in schedule
        /// </summary>
        /// <param name="item"></param>
        public void Add(Schedule item)
        {
            lock (_session)
            {
                _session.Store(item);
            }
        }

        /// <summary>
        /// Updates schedule item
        /// </summary>
        /// <param name="item"></param>
        public void Update(Schedule item)
        {
            lock (_session)
            {
                _session.Store(item);
            }
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
            return _session.GetAll<Schedule>(
                s => s.Date >= fromDate.Date
                    && s.Date < toDate.Date.AddDays(1)
                    && s.SalesArea.In(salesAreaNames),
                indexName: Schedules_ByIdAndBreakIdAndSalesAreaAndDate.DefaultIndexName,
                isMapReduce: false).ToList();
        }

        public List<BreakSimple> GetScheduleSimpleBreaks(List<string> salesAreaNames, DateTime fromDate, DateTime toDate) =>
            _session.GetAll<Schedule>(s => s.Date >= fromDate.Date
                        && s.Date < toDate.Date.AddDays(1)
                        && s.SalesArea.In(salesAreaNames),
                    Schedules_ByIdAndBreakIdAndSalesAreaAndDate.DefaultIndexName, false)
                .Where(b => b.Breaks != null)
                .SelectMany(s => s.Breaks.Select(b =>
                    new BreakSimple
                    {
                        SalesAreaName = b.SalesArea,
                        ScheduleDate = b.ScheduledDate.Date,
                        CustomId = b.CustomId,
                        ExternalBreakRef = b.ExternalBreakRef
                    }))
                .ToList();

        public List<BreakWithProgramme> GetBreakModels(List<string> salesAreaNames, DateTime fromDate, DateTime toDate, string excludeBreakType)
        {
            List<Schedule> schedules = _session
                .GetAll<Schedule>(
                    s => s.Date >= fromDate.Date
                        && s.Date <= toDate.Date.AddDays(1)
                        && s.SalesArea.In(salesAreaNames),
                    indexName: Schedules_ByIdAndBreakIdAndSalesAreaAndDate.DefaultIndexName,
                    isMapReduce: false);

            if (schedules.Count == 0)
            {
                return null;
            }

            var index = (from schedule in schedules.Where(s => s.Breaks != null && s.Breaks.Any())
                         let programmes = schedule.Programmes?.OrderBy(d => d.StartDateTime)

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
                             EpisodeNumber = pgm?.Episode?.Number,
                             PrgtNo = pgm?.PrgtNo ?? 0
                         }).ToList();

            return index;
        }

        public Schedule Get(int id)
        {
            lock (_session)
            {
                return _session.Load<Schedule>(id);
            }
        }

        public void Delete(int id)
        {
            lock (_session)
            {
                var item = Get(id);
                if (item is null)
                {
                    return;
                }

                _session.Delete(item);
            }
        }

        public List<Tuple<Break, Programme>> GetBreakWithProgramme(List<string> salesAreaNames, DateTime fromDate,
            DateTime toDate)
        {
            var schedules = _session.GetAll<Schedule>(s => s.Date >= fromDate.Date
                                                           && s.Date < toDate.Date
                                                               .AddDays(1)
                                                           && s.SalesArea.In(
                                                               salesAreaNames),
                    indexName: Schedules_ByIdAndBreakIdAndSalesAreaAndDate.DefaultIndexName, isMapReduce: false)
                .ToList();

            if (!schedules.Any())
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
                         let brks = breaks.EmptyIfNull().Where(b => pgm.StartDateTime <= b.ScheduledDate &&
                                                                    pgm.StartDateTime.AddTicks(pgm.Duration
                                                                        .BclCompatibleTicks) >=
                                                                    b.ScheduledDate).EmptyIfNull()
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
            // Wait for non-stale results because this method is used when
            // creating the Schedule document and would otherwise cause multiple
            // Schedule documents to be created for the same sales area & date.
            return _session.Query<Schedule>()
                 .Customize(x => x.WaitForNonStaleResults(DocumentSessionExtensions.LongWaitTimeoutForNonStaleIndexes))
                 .FirstOrDefault(s => s.SalesArea == salesAreaName && s.Date == date);
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
            var schedules = _session.GetAll<Schedule>(s => s.Date >= fromDate.Date
                                                         && s.Date < toDate.Date.AddDays(1)
                                                         && s.SalesArea.In(salesAreaNames),
                                                         Schedules_ByIdAndBreakIdAndSalesAreaAndDate.DefaultIndexName, false).ToList();

            return schedules.Count > 0
                ? schedules.Where(b => b.Breaks != null).SelectMany(s => s.Breaks)
                    .Where(x => x.ScheduledDate >= fromDate).Where(x => x.ScheduledDate <= toDate).ToList()
                : null;
        }

        public List<Programme> GetProgrammes(List<string> salesAreaNames, DateTime fromDate, DateTime toDate)
        {
            var schedules = _session.GetAll<Schedule>(s => s.Date >= fromDate.Date
                                                                  && s.Date < toDate.Date.AddDays(1)
                                                                  && s.SalesArea.In(salesAreaNames),
                                                                  indexName: Schedules_ByIdAndBreakIdAndSalesAreaAndDate.DefaultIndexName, isMapReduce: false).ToList();
            return schedules.Count > 0 ? (schedules.Where(b => b.Programmes != null).SelectMany(s => s.Programmes).ToList()) : null;
        }

        public List<Schedule> FindByBreakIds(IEnumerable<Guid> breakIds)
        {
            var schedules = new List<Schedule>();
            var groups = RavenRepositoryHelper.GroupElementsForInClause(breakIds);

            foreach (var itemGroup in groups)
            {
                schedules.AddRange(_session.GetAll<Schedule>(x => x.Breaks.Any(b => b.Id.In(itemGroup.ToList())),
                    indexName: Schedules_ByIdAndBreakIdAndSalesAreaAndDate.DefaultIndexName, isMapReduce: false));
            }

            return schedules.Distinct(new ScheduleByIdComparer()).ToList();
        }

        [Obsolete("Try to use TruncateAsync() as it is more reliable.")]
        public void Truncate()
        {
            _session.TryDelete("Raven/DocumentsByEntityName", "Schedules");
        }

        public async Task TruncateAsync()
        {
            const int maximumTimeoutSeconds = 180;
            const int retryMillisecondDelay = 100;
            const int maximumNumberOfRetries = 100;

            var maximumSecondsWaitingForNonStaleIndexes = TimeSpan.FromSeconds(maximumTimeoutSeconds);
            int remainingRetries = maximumNumberOfRetries;
            bool retry = false;
            var startTime = DateTime.UtcNow;

            do
            {
                retry = false;

                try
                {
                    var operation = await _sessionAsync.Advanced
                        .DeleteByIndexAsync<Schedule, Schedules_ByIdAndBreakIdAndSalesAreaAndDate>(ForceDelete())
                        .ConfigureAwait(false);

                    await operation
                        .WaitForCompletionAsync()
                        .ConfigureAwait(false);
                }
                catch (Exception ex) when (CanRetry(ex))
                {
                    remainingRetries--;
                    retry = true;

                    await Task.Delay(retryMillisecondDelay)
                        .ConfigureAwait(false);

                    continue;
                }
                catch (Exception ex) when (remainingRetries == 0)
                {
                    throw new RepositoryException(
                        $"Deleting all Schedule documents stopped after {maximumNumberOfRetries} attempts. Wait for a few minutes and try again.",
                        ex
                    );
                }
                catch (Exception ex) when (IndexIsStale(ex))
                {
                    throw new RepositoryException(
                        $"Deleting all Schedule documents timed out after {maximumTimeoutSeconds} seconds as the index is stale. Wait for a few minutes and try again.",
                        ex
                    );
                }
            } while (retry);

            bool IndexIsStale(Exception ex) => ex.Message.Contains("index is stale");

            bool MaximumTimeToWaitForIndexesExceeded() =>
                DateTime.UtcNow - startTime > maximumSecondsWaitingForNonStaleIndexes;

            bool CanRetry(Exception ex) =>
                IndexIsStale(ex) &&
                remainingRetries > 0 &&
                !MaximumTimeToWaitForIndexesExceeded();

            Expression<Func<Schedule, bool>> ForceDelete() =>
                schedule => schedule.Id != int.MinValue;
        }

        public void SaveChanges()
        {
            lock (_session)
            {
                _session.SaveChanges();
                WaitForIndexes();
            }

            void WaitForIndexes()
            {
                string fakeSalesAreaNameToForceIndexLookup = Guid.NewGuid().ToString();

                _session.WaitForIndexes<Schedule>(
                    Schedules_ByIdAndBreakIdAndSalesAreaAndDate.DefaultIndexName,
                    isMapReduce: false,
                    testExpression: schedule => schedule.SalesArea.Equals(fakeSalesAreaNameToForceIndexLookup)
                    );
            }
        }

        public async Task SaveChangesAsync() =>
            await _sessionAsync.SaveChangesAsync()
                .ConfigureAwait(false);
    }
}
