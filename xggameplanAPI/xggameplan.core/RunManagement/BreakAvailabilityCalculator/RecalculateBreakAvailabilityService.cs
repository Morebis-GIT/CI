using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.Shared.Schedules;
using ImagineCommunications.GamePlan.Domain.Spots;
using Microsoft.Extensions.Logging;
using xggameplan.common.Extensions;
using xggameplan.common.Helpers;
using xggameplan.core.Interfaces;

namespace xggameplan.core.RunManagement.BreakAvailabilityCalculator
{
    public class RecalculateBreakAvailabilityService : IRecalculateBreakAvailabilityService
    {
        private readonly ILogger _logger;
        private readonly IRepositoryFactory _repositoryFactory;

        private static readonly TimeSpan _defaultBroadcastDayEndTime = new TimeSpan(5, 59, 59);

        public RecalculateBreakAvailabilityService(
            IRepositoryFactory repositoryFactory,
            ILogger<IRecalculateBreakAvailabilityService> logger)
        {
            _repositoryFactory = repositoryFactory;
            _logger = logger;
        }

        public void Execute(DateTimeRange period, IEnumerable<SalesArea> salesAreas, CancellationToken cancellationToken = default)
        {
            if (!salesAreas.Any())
            {
                _logger.LogWarning("No sales areas were passed to the calculator.");
                return;
            }

            var salesAreaNames = salesAreas
                .Select(sa => sa.Name)
                .ToList();

            _logger.LogInformation($"Recalculating break availability for {LogAsString.Log(period.Start)} to {LogAsString.Log(period.End)}");

            var allSpotsSubsetForAllSalesAreasForRunPeriodCollection =
                GetAllSpotSubsetsForAllSalesAreasForPeriod(period, salesAreaNames);

            if (allSpotsSubsetForAllSalesAreasForRunPeriodCollection.Count == 0)
            {
                _logger.LogWarning(
                    $"Did not find any spots for the sales areas {string.Join("; ", salesAreaNames)} between {LogAsString.Log(period.Start)} to {LogAsString.Log(period.End)}"
                    );

                return;
            }

            var datesToProcess = GetDatesToProcess(period);

            var salesAreasToProcessInParallel = new ParallelOptions
            {
                MaxDegreeOfParallelism = Math.Max(Environment.ProcessorCount, 1)
            };

            var daysToProcessInParallel = new ParallelOptions
            {
                MaxDegreeOfParallelism = Math.Max(Environment.ProcessorCount / 2, 1)
            };

            _ = Parallel.ForEach(salesAreaNames, salesAreasToProcessInParallel, salesAreaName =>
            {
                _logger.LogInformation($"Calculating break availability and optimiser availability for sales area {salesAreaName}");

                var spotSubsetForSalesAreaCollection = ImmutableList.CreateRange(
                    allSpotsSubsetForAllSalesAreasForRunPeriodCollection
                        .Where(s => s.SalesArea == salesAreaName));

                var programmeSubsetForSalesAreaForRunPeriodCollection =
                    GetProgrammesSubsetForPeriodForSalesArea(period, salesAreaName);

                _ = Parallel.ForEach(datesToProcess, daysToProcessInParallel, date =>
                {
                    var programmesForUtcDate = programmeSubsetForSalesAreaForRunPeriodCollection
                        .Where(prog => prog.StartDateTime.Date == date.Date)
                        .ToImmutableList();

                    if (programmesForUtcDate.Count == 0)
                    {
                        _logger.LogWarning($"No programmes found for sales area {salesAreaName} on {LogAsString.Log(date.Date)}");
                        return;
                    }

                    _logger.LogInformation(
                        $"Found {LogAsString.Log(programmesForUtcDate.Count)} programmes " +
                        $"for sales area {salesAreaName} on {LogAsString.Log(date.Date)}. " +
                        $"(Programme Ids: {String.Join(",", programmesForUtcDate.Select(p => p.ProgrammeId))})"
                        );

                    var anyProgrammesSpanningMidnight = programmesForUtcDate
                        .Any(p => p.StartDateTime.Add(p.Duration.ToTimeSpan()) >= date.Date.AddDays(1));

                    Func<ISpotForBreakAvailCalculation, bool> condition;

                    if (anyProgrammesSpanningMidnight)
                    {
                        condition = spot =>
                            spot.StartDateTime.Date == date.Date ||
                            spot.StartDateTime.Date == date.Date.AddDays(1);
                    }
                    else
                    {
                        condition = spot => spot.StartDateTime.Date == date.Date;
                    }

                    var spotsForUtcDate = spotSubsetForSalesAreaCollection
                        .Where(condition)
                        .ToList();

                    if (spotsForUtcDate.Count > 0)
                    {
                        _logger.LogInformation(
                            $"Found {LogAsString.Log(spotsForUtcDate.Count)} spots " +
                            $"for sales area {salesAreaName} on {LogAsString.Log(date.Date)}"
                            );
                    }
                    else
                    {
                        _logger.LogWarning($"No spots found for sales area {salesAreaName} on {LogAsString.Log(date.Date)}");
                    }

                    using (var scope = _repositoryFactory.BeginRepositoryScope())
                    {
                        var breakRepository = scope.CreateRepository<IBreakRepository>()
                            ?? throw new NullReferenceException($"An instance of {nameof(IBreakRepository)} was not found.");

                        var dateTo = anyProgrammesSpanningMidnight
                            ? date.Date.AddDays(2).AddSeconds(-1)
                            : date.Date.AddDays(1).AddSeconds(-1);

                        var breaksForUtcDate = breakRepository.Search(
                            date.Date,
                            dateTo,
                            salesAreaName
                        ).ToList();

                        if (breaksForUtcDate.Count == 0)
                        {
                            _logger.LogWarning($"No breaks found for sales area {salesAreaName} on {LogAsString.Log(date.Date)}");
                            return;
                        }

                        _logger.LogInformation(
                            $"Found {LogAsString.Log(breaksForUtcDate.Count)} break(s) for sales area {salesAreaName} " +
                            $"on {LogAsString.Log(date.Date)}. " +
                            $"[Break Ext. Refs: {breaksForUtcDate.ReducePropertyToCsv(x => x.ExternalBreakRef)}]"
                        );

                        var scheduleRepository = scope.CreateRepository<IScheduleRepository>()
                            ?? throw new NullReferenceException($"An instance of {nameof(IScheduleRepository)} was not found.");

                        Schedule scheduleForUtcDate = GetScheduleForUtcDate(
                            salesAreaName,
                            date,
                            scheduleRepository);

                        CalculateBreaksAvailsForUtcDate(
                            salesAreaName,
                            programmesForUtcDate,
                            spotsForUtcDate,
                            breaksForUtcDate,
                            scheduleForUtcDate,
                            breakRepository,
                            scheduleRepository);

                        if (anyProgrammesSpanningMidnight)
                        {
                            var programmesSpanningMidnight = programmesForUtcDate
                                .Where(p => p.StartDateTime.Add(p.Duration.ToTimeSpan()) >= date.Date)
                                .ToList();

                            CopyUpdatedPostMidnightBreakAvails(
                                salesAreaName,
                                date.AddDays(1),
                                programmesSpanningMidnight,
                                breaksForUtcDate,
                                breakRepository,
                                scheduleRepository);
                        }

                        breakRepository.SaveChanges();
                        scheduleRepository.SaveChanges();
                    }
                });
            });
        }

        /// <summary>
        /// These breaks have already been calculated. The schedule breaks for
        /// the following day need updating as they're in a different Schedule.
        /// </summary>
        /// <param name="salesAreaName"></param>
        /// <param name="date"></param>
        /// <param name="programmesSpanningMidnight"></param>
        /// <param name="breaksForUtcDate"></param>
        /// <param name="breakRepository"></param>
        /// <param name="scheduleRepository"></param>
        private void CopyUpdatedPostMidnightBreakAvails(
            string salesAreaName,
            DateTime date,
            IReadOnlyList<IProgrammeForBreakAvailCalculation> programmesSpanningMidnight,
            IReadOnlyList<Break> breaksForUtcDate,
            IBreakRepository breakRepository,
            IScheduleRepository scheduleRepository)
        {
            if (programmesSpanningMidnight.Count == 0)
            {
                return;
            }

            var scheduleForUtcDatePlusOne = GetScheduleForUtcDate(
                salesAreaName,
                date,
                scheduleRepository);

            if (scheduleForUtcDatePlusOne is null)
            {
                return;
            }

            var breakUpdateHandler = new BreakAvailabilityUpdateHandler(
                _logger,
                breakRepository,
                scheduleForUtcDatePlusOne);

            foreach (var programme in programmesSpanningMidnight)
            {
                var breaksAfterMidnight = breaksForUtcDate
                    .Where(theBreak => programme.DateTimeIsInProgramme(theBreak.ScheduledDate));

                if (!breaksAfterMidnight.Any())
                {
                    continue;
                }

                foreach (var oneBreak in breaksAfterMidnight)
                {
                    breakUpdateHandler.UpdateAvailability(oneBreak);
                    breakUpdateHandler.UpdateOptimizerAvailability(oneBreak);
                    scheduleRepository.Update(scheduleForUtcDatePlusOne);
                }
            }
        }

        private void CalculateBreaksAvailsForUtcDate(
             string salesAreaName,
             ImmutableList<IProgrammeForBreakAvailCalculation> programmes,
             IReadOnlyList<ISpotForBreakAvailCalculation> spots,
             IReadOnlyList<Break> breaks,
             Schedule scheduleForUtcDate,
             IBreakRepository breakRepository,
             IScheduleRepository scheduleRepository)
        {
            var breakUpdateHandler = new BreakAvailabilityUpdateHandler(
                _logger,
                breakRepository,
                scheduleForUtcDate);

            new BreakAndOptimiserAvailabilityCalculator<Break>(_logger, breakUpdateHandler)
                .Calculate(
                    salesAreaName,
                    programmes,
                    breaks,
                    spots
                );

            if (scheduleForUtcDate is null || !breakUpdateHandler.ScheduleBreaksHaveChanges)
            {
                return;
            }

            scheduleRepository.Update(scheduleForUtcDate);
        }

        /// <summary>
        /// Get the schedule for the given date or returns null.
        /// </summary>
        /// <param name="salesAreaName"></param>
        /// <param name="date"></param>
        /// <param name="scheduleRepository"></param>
        /// <returns></returns>
        private Schedule GetScheduleForUtcDate(
            string salesAreaName,
            DateTime date,
            IScheduleRepository scheduleRepository)
        {
            var result = scheduleRepository.GetSchedule(salesAreaName, date.Date);
            if (result is null)
            {
                _logger.LogWarning($"No schedule found for sales area {salesAreaName} on {LogAsString.Log(date.Date)}");
            }
            else
            {
                _logger.LogInformation(
                    $"Found schedule for sales area {salesAreaName} on " +
                    $"{LogAsString.Log(date.Date)} (Schedule Id: {LogAsString.Log(result.Id)})"
                );
            }

            return result;
        }

        private static IImmutableList<DateTime> GetDatesToProcess(DateTimeRange period)
        {
            var individualDates = new List<DateTime>();
            DateTime date;

            for (date = period.Start; date <= period.End; date = date.AddDays(1))
            {
                individualDates.Add(date);
            }

            if (period.End.TimeOfDay <= _defaultBroadcastDayEndTime)
            {
                if (!individualDates.Contains(date))
                {
                    individualDates.Add(date);
                }
            }

            return ImmutableList.CreateRange(individualDates);
        }

        private ImmutableList<ISpotForBreakAvailCalculation> GetAllSpotSubsetsForAllSalesAreasForPeriod(
            DateTimeRange period, List<string> salesAreaNames)
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var repository = scope.CreateRepository<ISpotRepository>()
                ?? throw new NullReferenceException($"An instance of {nameof(ISpotRepository)} was not found.");

            return repository
                .Search(period.Start, period.End, salesAreaNames)
                .Select(spot => new SpotSubsetForBreakAvailCalculation(spot) as ISpotForBreakAvailCalculation)
                .ToImmutableList();
        }

        private ImmutableList<IProgrammeForBreakAvailCalculation> GetProgrammesSubsetForPeriodForSalesArea(
            DateTimeRange period, string salesAreaName)
        {
            using var scope = _repositoryFactory.BeginRepositoryScope();
            var repository = scope.CreateRepository<IProgrammeRepository>()
                ?? throw new NullReferenceException($"An instance of {nameof(IProgrammeRepository)} was not found.");

            return repository
                .Search(period.Start, period.End, salesAreaName)
                .Select(programme =>
                    new ProgrammeSubsetForBreakAvailCalculation(programme) as IProgrammeForBreakAvailCalculation)
                .ToImmutableList();
        }
    }
}
