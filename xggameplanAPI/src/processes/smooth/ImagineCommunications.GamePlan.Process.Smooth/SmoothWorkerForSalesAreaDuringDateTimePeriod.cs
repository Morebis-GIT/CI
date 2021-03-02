using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;
using ImagineCommunications.GamePlan.Domain.Generic.Repository;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Recommendations.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using ImagineCommunications.GamePlan.Domain.SmoothFailures;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;
using ImagineCommunications.GamePlan.Process.Smooth.Legacy;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Services;

namespace ImagineCommunications.GamePlan.Process.Smooth
{
    /// <summary>
    /// Executes Smooth for a specific sales area during a given date time period.
    /// </summary>
    public partial class SmoothWorkerForSalesAreaDuringDateTimePeriod
    {
        private readonly string _smoothLogFileFolder;
        private readonly IClashExposureCountService _clashExposureCountService;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly ISmoothConfiguration _smoothConfiguration;
        private readonly ImmutableSmoothData _threadSafeCollections;

        private Action<string> RaiseInfo { get; }
        private Action<string> RaiseWarning { get; }
        private Action<string, Exception> RaiseException { get; }

        // Notification of day processed
        public delegate void SmoothBatchComplete(
            object sender,
            DateTime fromDateTime,
            DateTime toDateTime,
            List<Recommendation> recommendations,
            List<SmoothFailure> smoothFailures);

        /// <summary>
        /// An event raised when a batch of days has been Smoothed.
        /// </summary>
        public event SmoothBatchComplete OnSmoothBatchComplete;

        public SmoothWorkerForSalesAreaDuringDateTimePeriod(
            IRepositoryFactory repositoryFactory,
            string smoothLogFileFolder,
            ImmutableSmoothData threadSafeCollections,
            IClashExposureCountService clashExposureCountService,
            Action<string> raiseInfo,
            Action<string> raiseWarning,
            Action<string, Exception> raiseException
            )
        {
            _smoothLogFileFolder = smoothLogFileFolder;
            _clashExposureCountService = clashExposureCountService;
            _repositoryFactory = repositoryFactory;
            _smoothConfiguration = threadSafeCollections.SmoothConfigurationReader;
            _threadSafeCollections = threadSafeCollections;

            RaiseInfo = raiseInfo;
            RaiseWarning = raiseWarning;
            RaiseException = raiseException;
        }

        public SmoothOutput ActuallyStartSmoothing(
            Guid runId,
            Guid firstScenarioId,
            DateTime processorDateTime,
            DateTimeRange smoothPeriod,
            SalesArea salesArea)
        {
            ISmoothDiagnostics smoothDiagnostics = new FileSmoothDiagnostics(
                runId,
                salesArea.Name,
                processorDateTime,
                _smoothLogFileFolder,
                _threadSafeCollections.SmoothConfigurationReader);

            var smoothOutput = new SmoothOutput { SalesAreaName = salesArea.Name };

            foreach (var item in PrepareSmoothFailureMessageCollection(_threadSafeCollections.SmoothFailureMessages))
            {
                smoothOutput.SpotsByFailureMessage.Add(item);
            }

            IImmutableList<SmoothPass> smoothPasses = _smoothConfiguration.SortedSmoothPasses;
            foreach (var item in PrepareSmoothOutputWithSmoothPasses(smoothPasses))
            {
                smoothOutput.OutputByPass.Add(item);
            }

            var (fromDateTime, toDateTime) = smoothPeriod;
            if (toDateTime.TimeOfDay.Ticks == 0) // No time part, include whole day
            {
                toDateTime = toDateTime.AddDays(1).AddTicks(-1);
            }

            var weekBatchesToProcessInParallel = new ParallelOptions
            {
                MaxDegreeOfParallelism = 2
            };

            var smoothFailuresFactory = new SmoothFailuresFactory(_smoothConfiguration);
            var smoothRecommendationsFactory = new SmoothRecommendationsFactory(_smoothConfiguration);
            var batchAllThreadOutputCollection = new ConcurrentBag<SmoothBatchOutput>();

            var _saveSmoothChanges = new SaveSmoothChanges(
                _repositoryFactory,
                _threadSafeCollections,
                RaiseInfo);

            var smoothDateRange = new SmoothDateRange(
                runId,
                firstScenarioId,
                processorDateTime,
                salesArea,
                _smoothConfiguration,
                smoothDiagnostics,
                _threadSafeCollections,
                _clashExposureCountService,
                _saveSmoothChanges,
                _repositoryFactory,
                RaiseInfo,
                RaiseWarning,
                RaiseException);

            _ = Parallel.ForEach(
                    DateHelper.SplitUTCDateRange((fromDateTime, toDateTime), 7),
                    weekBatchesToProcessInParallel,
                    dateRangeToSmooth =>
                    {
                        var result = smoothDateRange.Execute(dateRangeToSmooth);

                        OnSmoothBatchComplete?.Invoke(
                            this,
                            dateRangeToSmooth.Start,
                            dateRangeToSmooth.End,
                            result.recommendations,
                            result.smoothFailures
                            );

                        batchAllThreadOutputCollection.Add(result.smoothBatchOutput);
                    });

            var (spotIdsUsed, spotIdsNotUsed) = CollateThreadOutputToSmoothOutput(
                smoothOutput,
                batchAllThreadOutputCollection);

            _saveSmoothChanges.SaveUnplacedSpots(
                smoothOutput,
                spotIdsNotUsed,
                spotIdsUsed,
                smoothDiagnostics
                );

            return smoothOutput;
        }

        private static IDictionary<int, int> PrepareSmoothFailureMessageCollection(
            IImmutableList<SmoothFailureMessage> smoothFailureMessages)
        {
            var result = new Dictionary<int, int>();

            foreach (var message in smoothFailureMessages)
            {
                result.Add(message.Id, 0);
            }

            return result;
        }

        private static IDictionary<int, SmoothOutputForPass> PrepareSmoothOutputWithSmoothPasses(
            IReadOnlyCollection<SmoothPass> smoothPasses
            )
        {
            var result = new Dictionary<int, SmoothOutputForPass>();

            foreach (var smoothPass in smoothPasses)
            {
                if (result.ContainsKey(smoothPass.Sequence))
                {
                    continue;
                }

                result.Add(
                    smoothPass.Sequence,
                    new SmoothOutputForPass
                    {
                        PassSequence = smoothPass.Sequence
                    });
            }

            return result;
        }
    }
}
