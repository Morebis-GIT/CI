using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Persistence.SqlServer.BusinessLogic.BreakAvailabilityCalculator.Exceptions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.BusinessLogic.BreakAvailabilityCalculator.Objects;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Breaks;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Programmes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using xggameplan.common.Extensions;
using xggameplan.common.Helpers;
using xggameplan.core.RunManagement.BreakAvailabilityCalculator;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.BusinessLogic.BreakAvailabilityCalculator
{
    public class BreakAvailabilityCalculator : IDisposable
    {
        private readonly ILogger _logger;
        private readonly ISqlServerDbContextFactory<ISqlServerLongRunningTenantDbContext> _tenantDbContextFactory;
        private readonly RecalculateBreakAvailabilityOptions _recalculateOptions;
        private readonly CancellationToken _cancellationToken;
        private readonly TransformManyBlock<IBreakAvailability[], IBreakAvailability> _internalBlock;
        private readonly SemaphoreSlim _lockDbContext;

        private bool _disposed;

        public BreakAvailabilityCalculator(
            ILogger logger,
            ISqlServerDbContextFactory<ISqlServerLongRunningTenantDbContext> tenantDbContextFactory,
            RecalculateBreakAvailabilityOptions recalculateOptions,
            ITargetBlock<IBreakAvailability> targetBlock,
            CancellationToken cancellationToken)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tenantDbContextFactory = tenantDbContextFactory ?? throw new ArgumentNullException(nameof(tenantDbContextFactory));
            _recalculateOptions = recalculateOptions ?? throw new ArgumentNullException(nameof(recalculateOptions));
            _cancellationToken = cancellationToken;

            _internalBlock = new TransformManyBlock<IBreakAvailability[], IBreakAvailability>(
                breaks => breaks.AsEnumerable(), new ExecutionDataflowBlockOptions
                {
                    CancellationToken = _cancellationToken
                });
            _ = _internalBlock.LinkTo(targetBlock ?? throw new ArgumentNullException(nameof(targetBlock)));
            _ = _internalBlock.Completion.ContinueWith(_ => targetBlock.Complete());

            _lockDbContext = new SemaphoreSlim(_recalculateOptions.BoundedDbContextOperationCapacity,
                _recalculateOptions.BoundedDbContextOperationCapacity);
        }

        public async Task CalculateAsync(DateTimeRange period, string salesAreaName)
        {
            var hasErrors = false;
            _logger.LogInformation(
                $"Break availability calculation for '{salesAreaName}' sales area " +
                $"on {LogAsString.Log(period.Start.Date)} has been started.");

            try
            {
                List<IProgrammeForBreakAvailCalculation> programmes = await
                    GetProgrammesAsync(period, salesAreaName)
                        .ConfigureAwait(false);

                if (programmes.Count == 0)
                {
                    _logger.LogWarning(
                        $"No programmes found for sales area {salesAreaName} " +
                        $"on {LogAsString.Log(period.Start.Date)}");

                    return;
                }

                DateTimeRange periodCoveringWholeProgrammes =
                    PeriodIncludingAnyProgrammeSpanningMidnight(period, programmes);

                var spotsQueryTask = GetSpots(periodCoveringWholeProgrammes, salesAreaName);
                var breaksQueryTask = GetBreaks(periodCoveringWholeProgrammes, salesAreaName);

                await Task
                    .WhenAll(spotsQueryTask, breaksQueryTask)
                    .ConfigureAwait(false);

                List<ISpotForBreakAvailCalculation> spots = spotsQueryTask.Result;
                if (spots.Count > 0)
                {
                    _logger.LogInformation(
                        $"Found {LogAsString.Log(spots.Count)} spots "
                        + $"for sales area {salesAreaName} " +
                        $"on {LogAsString.Log(period.Start.Date)}"
                    );
                }
                else
                {
                    _logger.LogWarning(
                        $"No spots found for sales area {salesAreaName} " +
                        $"on {LogAsString.Log(period.Start.Date)}");
                }

                _logger.LogInformation(
                    $"Found {LogAsString.Log(programmes.Count)} programmes "
                    + $"for sales area {salesAreaName} on {LogAsString.Log(period.Start.Date)}. "
                    + $"(Programme Ids: {programmes.ReducePropertyToCsv(p => p.ProgrammeId)})"
                );

                List<BreakAvailability> breaks = breaksQueryTask.Result;
                if (breaks.Count > 0)
                {
                    string breakExternalRefs = breaks.ReducePropertyToCsv(x => x.ExternalBreakRef);

                    _logger.LogInformation(
                        $"Found {LogAsString.Log(breaks.Count)} break(s) for sales area {salesAreaName} "
                        + $"on {LogAsString.Log(period.Start.Date)}. "
                        + $"[Break Ext. Refs: {breakExternalRefs}]"
                    );
                }
                else
                {
                    _logger.LogWarning(
                        $"No breaks found for sales area {salesAreaName} " +
                        $"on {LogAsString.Log(period.Start.Date)}");
                }

                var updateBreakHandler = new BreakAvailabilityUpdateHandler();
                var calculator = new BreakAndOptimiserAvailabilityCalculator<BreakAvailability>(_logger, updateBreakHandler);
                calculator.Calculate(salesAreaName, programmes, breaks, spots);

                if (updateBreakHandler.UpdatedBreaks.Count > 0)
                {
                    _ = await _internalBlock
                            .SendAsync(updateBreakHandler.UpdatedBreaks.Values.ToArray(), _cancellationToken)
                            .ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation(
                    $"Break availability calculation for '{salesAreaName}' sales area " +
                    $"on {LogAsString.Log(period.Start.Date)} has been cancelled.");
                hasErrors = true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Break availability calculation for '{salesAreaName}' sales area " +
                    $"on {LogAsString.Log(period.Start.Date)} has been finished with errors.");
                hasErrors = true;
                throw new BreakAvailabilityCalculatorException(period, salesAreaName, ex);
            }
            finally
            {
                if (!hasErrors)
                {
                    _logger.LogInformation(
                        $"Break availability calculation for '{salesAreaName}' sales area " +
                        $"on {LogAsString.Log(period.Start.Date)} has been finished successfully.");
                }
            }

            static bool ProgrammeThatFinishesAfterMidnight(
                DateTime startDate,
                Duration duration,
                DateTime endOfProgramme) =>
                startDate.Add(duration.ToTimeSpan()).Date == endOfProgramme.Date;

            static DateTimeRange PeriodIncludingAnyProgrammeSpanningMidnight(
                in DateTimeRange period,
                List<IProgrammeForBreakAvailCalculation> programmes)
            {
                var periodEnd = period.End;

                var programmeSpanningMidnight = programmes
                    .Find(p => ProgrammeThatFinishesAfterMidnight(
                        p.StartDateTime, p.Duration, periodEnd)
                    );

                if (programmeSpanningMidnight is null)
                {
                    return period;
                }

                var programmeEndTime = programmeSpanningMidnight
                    .StartDateTime
                    .Add(programmeSpanningMidnight.Duration.ToTimeSpan())
                    .TimeOfDay;

                var periodCoveringWholeProgrammes =
                    (period.Start, periodEnd.Add(programmeEndTime));

                return periodCoveringWholeProgrammes;
            }
        }

        public void Complete()
        {
            _internalBlock.Complete();
        }

        protected async Task<List<ISpotForBreakAvailCalculation>> GetSpots(
            DateTimeRange period,
            string salesAreaName)
        {
            await _lockDbContext.WaitAsync(_cancellationToken).ConfigureAwait(false);

            try
            {
                using var dbContext = _tenantDbContextFactory.Create();
                return await dbContext.Query<Spot>()
                    .Where(s =>
                        s.StartDateTime >= period.Start && s.StartDateTime < period.End &&
                        s.SalesArea.Name == salesAreaName)
                    .AsNoTracking()
                    .Select(s => (ISpotForBreakAvailCalculation)new SpotAvailability
                    {
                        ExternalBreakNo = s.ExternalBreakNo,
                        SalesArea = s.SalesArea.Name,
                        SpotLength = Duration.FromTimeSpan(s.SpotLength),
                        StartDateTime = s.StartDateTime,
                        ClientPicked = s.ClientPicked
                    })
                    .ToListAsync(_cancellationToken)
                    .ConfigureAwait(false);
            }
            finally
            {
                _ = _lockDbContext.Release();
            }
        }

        protected async Task<List<IProgrammeForBreakAvailCalculation>> GetProgrammesAsync(
            DateTimeRange period,
            string salesAreaName)
        {
            await _lockDbContext.WaitAsync(_cancellationToken).ConfigureAwait(false);

            try
            {
                using var dbContext = _tenantDbContextFactory.Create();
                return await dbContext.Query<Programme>().Include(x => x.ProgrammeDictionary)
                    .Where(p =>
                        p.StartDateTime >= period.Start && p.StartDateTime < period.End &&
                        p.SalesArea.Name == salesAreaName)
                    .AsNoTracking()
                    .Select(p => (IProgrammeForBreakAvailCalculation)new ProgrammeAvailability
                    {
                        ProgrammeId = p.Id,
                        StartDateTime = p.StartDateTime,
                        Duration = Duration.FromTimeSpan(p.Duration),
                        ExternalReference = p.ProgrammeDictionary.ExternalReference,
                        SalesArea = p.SalesArea.Name
                    })
                    .ToListAsync(_cancellationToken)
                    .ConfigureAwait(false);
            }
            finally
            {
                _ = _lockDbContext.Release();
            }
        }

        protected async Task<List<BreakAvailability>> GetBreaks(
            DateTimeRange period,
            string salesAreaName)
        {
            await _lockDbContext.WaitAsync(_cancellationToken).ConfigureAwait(false);

            try
            {
                using var dbContext = _tenantDbContextFactory.Create();
                return await dbContext.Query<Break>()
                    .Where(b => b.ScheduledDate >= period.Start && b.ScheduledDate < period.End &&
                                b.SalesArea.Name == salesAreaName)
                    .AsNoTracking()
                    .Select(b => new BreakAvailability
                    {
                        Id = b.Id,
                        ExternalBreakRef = b.ExternalBreakRef,
                        ScheduledDate = b.ScheduledDate,
                        Duration = Duration.FromTimeSpan(b.Duration),
                        Avail = Duration.FromTimeSpan(b.Avail),
                        OptimizerAvail = Duration.FromTimeSpan(b.OptimizerAvail)
                    })
                    .ToListAsync(_cancellationToken)
                    .ConfigureAwait(false);
            }
            finally
            {
                _ = _lockDbContext.Release();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            if (disposing)
            {
                _internalBlock.Complete();
                _lockDbContext?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
