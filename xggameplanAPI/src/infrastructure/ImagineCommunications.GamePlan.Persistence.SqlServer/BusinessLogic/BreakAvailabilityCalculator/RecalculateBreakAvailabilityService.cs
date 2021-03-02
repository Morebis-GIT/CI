using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Persistence.SqlServer.BusinessLogic.BreakAvailabilityCalculator.Exceptions;
using ImagineCommunications.GamePlan.Persistence.SqlServer.Core.Interfaces;
using Microsoft.Extensions.Logging;
using xggameplan.common.Extensions;
using xggameplan.common.Helpers;
using xggameplan.core.Interfaces;

namespace ImagineCommunications.GamePlan.Persistence.SqlServer.BusinessLogic.BreakAvailabilityCalculator
{
    public class RecalculateBreakAvailabilityService : IRecalculateBreakAvailabilityService
    {
        private readonly ILogger<IRecalculateBreakAvailabilityService> _logger;
        private readonly ISqlServerDbContextFactory<ISqlServerLongRunningTenantDbContext> _tenantDbContextFactory;
        private readonly RecalculateBreakAvailabilityOptions _recalculateOptions;

        private static readonly TimeSpan _defaultBroadcastDayEndTime = new TimeSpan(5, 59, 59);

        public RecalculateBreakAvailabilityService(
            ILogger<IRecalculateBreakAvailabilityService> logger,
            ISqlServerDbContextFactory<ISqlServerLongRunningTenantDbContext> tenantDbContextFactory,
            RecalculateBreakAvailabilityOptions recalculateOptions)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tenantDbContextFactory = tenantDbContextFactory ?? throw new ArgumentNullException(nameof(tenantDbContextFactory));
            _recalculateOptions = recalculateOptions ?? throw new ArgumentNullException(nameof(recalculateOptions));
        }

        public void Execute(
            DateTimeRange period,
            IEnumerable<SalesArea> salesAreas,
            CancellationToken cancellationToken = default)
        {
            if (!salesAreas.Any())
            {
                _logger.LogWarning("No sales areas found");
                return;
            }

            var salesAreaNames = salesAreas.Select(x => x.Name).ToArray();

            _logger.LogInformation($"Recalculating of break availability for {LogAsString.Log(period.Start)} to {LogAsString.Log(period.End)} started.");
            var hasErrors = false;

            try
            {
                Task.Run(async () =>
                {
                    var exceptionList = new List<Exception>();

                    var propagatorBlock = new BatchBlock<IBreakAvailability>(
                        _recalculateOptions.UpdateBreakBatchSize,
                        new GroupingDataflowBlockOptions
                        {
                            CancellationToken = cancellationToken
                        });

                    using var calculator = new BreakAvailabilityCalculator(
                        _logger,
                        _tenantDbContextFactory,
                        _recalculateOptions,
                        propagatorBlock,
                        cancellationToken);

                    var updater = new BreakAvailabilityUpdater(
                        _logger,
                        _tenantDbContextFactory,
                        _recalculateOptions,
                        cancellationToken);

                    var calculationBlock = new ActionBlock<(DateTimeRange Period, string SalesAreaName)>(tuple =>
                        calculator.CalculateAsync(tuple.Period, tuple.SalesAreaName), new ExecutionDataflowBlockOptions
                        {
                            CancellationToken = cancellationToken,
                            MaxDegreeOfParallelism = DataflowBlockOptions.Unbounded,
                            BoundedCapacity = _recalculateOptions.BoundedCalculateTaskCapacity,
                        });

                    _ = updater.Start(propagatorBlock, () => calculationBlock.Complete());

                    try
                    {
                        var days = (period.End - period.Start).Days;
                        if(period.End.TimeOfDay <= _defaultBroadcastDayEndTime)
                        {
                            days++;
                        }
                        try
                        {
                            foreach (var tuple in Enumerable.Range(0, days + 1)
                                .Select(d => new DateTimeRange(period.Start.Date.AddDays(d), period.Start.Date.AddDays(d + 1)))
                                .SelectMany(p => salesAreaNames.Select(salesAreaName => (Period: p, SalesAreaName: salesAreaName))))
                            {
                                if (calculationBlock.Completion.IsCompleted)
                                {
                                    break;
                                }

                                _ = await calculationBlock
                                    .SendAsync(tuple, cancellationToken)
                                    .ConfigureAwait(false);
                            }
                        }
                        finally

                        {
                            calculationBlock.Complete();
                            exceptionList.AddRange(
                                await calculationBlock.Completion
                                    .WaitWithTaskExceptionGatheringAsync()
                                    .ConfigureAwait(false)
                            );
                        }
                    }
                    finally
                    {
                        calculator.Complete();
                        exceptionList.AddRange(await updater.WaitAsync().WaitWithTaskExceptionGatheringAsync()
                            .ConfigureAwait(false));
                    }

                    if (exceptionList.Count > 0)
                    {
                        await Task.FromException(new AggregateException(exceptionList))
                            .ConfigureAwait(false);
                    }
                }, cancellationToken).GetAwaiter().GetResult();
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation(
                    $"Recalculating of break availability for {LogAsString.Log(period.Start)} to {LogAsString.Log(period.End)} cancelled.");
                hasErrors = true;
            }
            catch (Exception ex)
            {
                var message =
                    $"Recalculating of break availability for {LogAsString.Log(period.Start)} to {LogAsString.Log(period.End)} finished with errors.";
                _logger.LogError(message, ex);
                hasErrors = true;
                throw new RecalculateBreakAvailabilityServiceException(message, ex);
            }
            finally
            {
                if (!hasErrors)
                {
                    _logger.LogInformation(
                        $"Recalculating of break availability for {LogAsString.Log(period.Start)} to {LogAsString.Log(period.End)} finished successfully.");
                }
            }
        }
    }
}
