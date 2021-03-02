using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Generic;
using ImagineCommunications.GamePlan.Domain.Generic.Helpers;
using ImagineCommunications.GamePlan.Domain.Generic.Types.Ranges;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.SalesAreas;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Services;
using Newtonsoft.Json;
using NodaTime.Extensions;
using xggameplan.common.Types;
using static xggameplan.common.Helpers.LogAsString;

namespace ImagineCommunications.GamePlan.Process.Smooth
{
    public class SmoothProcessor : ISmoothProcessor
    {
        private readonly RootFolder _rootFolder;
        private readonly IModelLoaders _modelLoaders;
        private readonly SmoothEngine _smoothEngine;

        public SmoothProcessor(
            SmoothEngine smoothEngine,
            IModelLoaders modelLoaders,
            RootFolder rootFolder
            )
        {
            _smoothEngine = smoothEngine;
            _rootFolder = rootFolder;
            _modelLoaders = modelLoaders;
        }

        public void Execute(
            Run run,
            IReadOnlyList<SalesArea> salesAreas,
            Action<string> raiseInfo,
            Action<string> raiseWarning,
            Action<string, Exception> raiseException
            )
        {
            if (raiseInfo is null)
            {
                throw new ArgumentNullException(nameof(raiseInfo));
            }

            if (raiseWarning is null)
            {
                throw new ArgumentNullException(nameof(raiseWarning));
            }

            if (raiseException is null)
            {
                throw new ArgumentNullException(nameof(raiseException));
            }

            if (run is null)
            {
                raiseWarning("Run passed to Smooth as null");
                return;
            }

            if (salesAreas is null)
            {
                raiseWarning($"Sales area list passed to Smooth as null for (RunID={Log(run.Id)})");
                return;
            }

            if (salesAreas.Count == 0)
            {
                raiseWarning($"No sales areas to execute Smooth for (RunID={Log(run.Id)})");
                return;
            }

            IReadOnlyCollection<SmoothStatistics> smoothStatistics = PrepareSmoothStatisticsForSalesAreas(salesAreas);

            var smoothOutputs = new List<SmoothOutput>();
            int smoothInstancesFailed = 0;

            var smoothStopWatch = new Stopwatch();
            smoothStopWatch.Start();

            try
            {
                DateTimeRange runPeriod = (
                    DateHelper.CreateStartDateTime(run.StartDate, run.StartTime),
                    DateHelper.CreateEndDateTime(run.EndDate, run.EndTime)
                    );

                DateTime processorDateTime = DateTime.UtcNow;

                DateTimeRange smoothPeriod = (
                    DateHelper.CreateStartDateTime(run.SmoothDateRange.Start, run.StartTime),
                    DateHelper.CreateEndDateTime(run.SmoothDateRange.End, run.EndTime)
                    );

                IReadOnlyCollection<string> salesAreaNames = new List<string>(
                    salesAreas.Select(s => s.Name)
                    );

                ImmutableSmoothData threadSafeCollections = LoadSmoothData(
                    _modelLoaders,
                    salesAreaNames,
                    smoothPeriod,
                    raiseInfo);

                raiseInfo($"Smoothing {Log(salesAreas.Count)} sales areas (RunID={Log(run.Id)})");

                LogSmoothConfiguration(
                    run.Id,
                    threadSafeCollections.SmoothConfiguration,
                    _rootFolder,
                    processorDateTime,
                    runPeriod,
                    smoothPeriod,
                    raiseInfo,
                    raiseWarning
                    );

                using (var processSmoothOutputMutex = new Mutex())
                {
                    _smoothEngine.OnSmoothBatchComplete += (
                        sender,
                        salesArea,
                        currentFromDateTime,
                        currentToDateTime,
                        dayRecommendations,
                        smoothFailures) =>
                    {
                        _ = processSmoothOutputMutex.WaitOne();

                        var timestamp = $"{Log(currentFromDateTime)} - {Log(currentToDateTime)} (RunID={Log(run.Id)})";

                        try
                        {
                            raiseInfo($"Processed Smooth output for sales area {salesArea.Name} for {timestamp}");
                        }
                        catch (Exception exception)
                        {
                            raiseException(
                                $"Error processing Smooth output for sales area {salesArea.Name} for {timestamp}",
                                exception
                                );
                        }
                        finally
                        {
                            processSmoothOutputMutex.ReleaseMutex();
                        }
                    };

                    // Define handler for Smooth output.
                    _smoothEngine.OnSmoothComplete += (
                        sender,
                        salesArea,
                        exception,
                        smoothOutput) =>
                    {
                        smoothOutputs.Add(smoothOutput);

                        var salesAreaName = salesArea.Name;

                        if (exception is null)
                        {
                            SmoothStatistics smoothStatistic = smoothStatistics.First(ss => ss.SalesAreaName == salesAreaName);

                            smoothStatistic.TimeEnded = DateTime.UtcNow;
                            TimeSpan elapsedTime = smoothStatistic.TimeEnded - smoothStatistic.TimeStarted;

                            var info = new StringBuilder(320);
                            _ = info
                                .Append("Completed Smooth processing")
                                .Append(": RunID=" + Log(run.Id))
                                .Append(": Sales Area=" + salesAreaName)
                                .Append(": Started=" + Log(smoothStatistic.TimeStarted))
                                .Append(": Ended=" + Log(smoothStatistic.TimeEnded))
                                .Append(": Elapsed=" + Log((long)elapsedTime.TotalSeconds) + "s")
                                .Append(": Breaks=" + smoothOutput.Breaks.ToString())
                                .Append(": Spots set=" + smoothOutput.SpotsSet.ToString())
                                .Append(": Spots not set=" + smoothOutput.SpotsNotSet.ToString())
                                .Append(": Recommendations=" + smoothOutput.Recommendations.ToString())
                                ;

                            raiseInfo(info.ToString());

                            return;
                        }

                        try
                        {
                            _ = processSmoothOutputMutex.WaitOne();

                            smoothInstancesFailed++;

                            raiseException(
                                $"Error executing Smooth for sales area {salesAreaName} (RunID={Log(run.Id)})",
                                exception);

                            if (exception is AggregateException aggEx)
                            {
                                AggregateException flatEx = aggEx.Flatten();

                                raiseException(
                                    $"Guru meditation: Exception during Smooth for sales area {salesAreaName} (RunID={Log(run.Id)})",
                                    flatEx
                                    );
                            }
                            else
                            {
                                raiseException(
                                    $"Guru meditation: Exception during Smooth for sales area {salesAreaName} (RunID={Log(run.Id)})",
                                    exception
                                    );

                                if (exception.InnerException != null)
                                {
                                    raiseException(
                                        $"Guru meditation: Exception during Smooth for sales area {salesAreaName} (RunID={Log(run.Id)})",
                                        exception
                                        );
                                }
                            }
                        }
                        catch (Exception exception2)
                        {
                            raiseException(
                                $"Guru meditation: Exception during outputting Smooth exception for sales area {salesArea.Name} (RunID={Log(run.Id)})",
                                exception2
                                );
                        }
                        finally
                        {
                            processSmoothOutputMutex.ReleaseMutex();
                        }
                    };

                    _ = Parallel.ForEach(salesAreas, salesArea =>
                    {
                        string salesAreaName = salesArea.Name;

                        raiseInfo($"Executing Smooth for sales area {salesAreaName} (RunID={Log(run.Id)})");

                        SmoothStatistics smoothStatistic = smoothStatistics.First(ss => ss.SalesAreaName == salesAreaName);
                        smoothStatistic.TimeStarted = DateTime.UtcNow;

                        _smoothEngine.SmoothSalesAreaForDateTimePeriod(
                            run.Id,
                            run.Scenarios[0].Id,
                            salesArea,
                            processorDateTime,
                            smoothPeriod,
                            threadSafeCollections,
                            raiseInfo,
                            raiseWarning,
                            raiseException
                            );
                    });
                }
            }
            finally
            {
                smoothStopWatch.Stop();

                var allSmoothOutput = new SmoothOutput()
                {
                    SpotsByFailureMessage = new Dictionary<int, int>()
                };

                foreach (var smoothOutput in smoothOutputs.Where(o => o != null))
                {
                    allSmoothOutput.Append(smoothOutput);

                    SmoothStatistics smoothStatistic = smoothStatistics.First(ss => ss.SalesAreaName == smoothOutput.SalesAreaName);
                    TimeSpan elapsedTimeSalesArea = smoothStatistic.TimeEnded - smoothStatistic.TimeStarted;

                    var passDetails = new StringBuilder();

                    foreach (var passSequence in smoothOutput.OutputByPass.Keys)
                    {
                        _ = passDetails
                            .AppendFormat("(Pass={0}, ", smoothOutput.OutputByPass[passSequence].PassSequence.ToString())
                            .AppendFormat("Spots set={0}); ", smoothOutput.OutputByPass[passSequence].CountSpotsSet.ToString());
                    }

                    // Remove the trailing space from the string.
                    passDetails.Length--;

                    // Log final statistics
                    var finalStats = new StringBuilder(800);
                    _ = finalStats
                        .Append("Smooth results")
                        .Append(": RunID=" + run.Id.ToString())
                        .Append(": Sales area=" + smoothOutput.SalesAreaName)
                        .Append(": Started=" + Log(smoothStatistic.TimeStarted))
                        .Append(": Ended=" + Log(smoothStatistic.TimeEnded))
                        .Append(": Elapsed=" + Log((long)elapsedTimeSalesArea.TotalSeconds) + "s")
                        .Append(": Breaks=" + smoothOutput.Breaks.ToString())
                        .Append(": Breaks with reduced Optimizer availability for unplaced spots=" + smoothOutput.BreaksWithReducedOptimizerAvailForUnplacedSpots.ToString())
                        .Append(": Spots set=" + smoothOutput.SpotsSet.ToString())
                        .Append(": Spots set after moving other spots=" + smoothOutput.SpotsSetAfterMovingOtherSpots.ToString())
                        .Append(": Spots not set=" + smoothOutput.SpotsNotSet.ToString())
                        .Append(": Spots not set due to excluded campaign=" + smoothOutput.SpotsNotSetDueToExternalCampaignRef.ToString())
                        .Append(": Booked spots unplaced due to restrictions=" + smoothOutput.BookedSpotsUnplacedDueToRestrictions.ToString())
                        .Append(": Recommendations=" + smoothOutput.Recommendations.ToString())
                        .Append(": Failures=" + smoothOutput.Failures.ToString())
                        .Append(": Passes Results=" + passDetails.ToString())
                        ;

                    raiseInfo(finalStats.ToString());
                }

                LogSmoothFailureMessages(allSmoothOutput.SpotsByFailureMessage, raiseInfo);

                var completedSmoothStats = new StringBuilder(512);
                _ = completedSmoothStats
                    .Append("Completed Smooth")
                    .Append(": RunID=" + Log(run.Id))
                    .Append(": Elapsed=" + Log(smoothStopWatch.ElapsedDuration()) + "s")
                    .Append(": Breaks=" + allSmoothOutput.Breaks.ToString())
                    .Append(": Breaks with reduced Optimizer availability for unplaced spots=" + allSmoothOutput.BreaksWithReducedOptimizerAvailForUnplacedSpots.ToString())
                    .Append(": Spots set=" + allSmoothOutput.SpotsSet.ToString())
                    .Append(": Spots set after moving other spots=" + allSmoothOutput.SpotsSetAfterMovingOtherSpots.ToString())
                    .Append(": Spots not set=" + allSmoothOutput.SpotsNotSet.ToString())
                    .Append(": Spots not set due to excluded campaign=" + allSmoothOutput.SpotsNotSetDueToExternalCampaignRef.ToString())
                    .Append(": Booked spots unplaced due to restrictions=" + allSmoothOutput.BookedSpotsUnplacedDueToRestrictions.ToString())
                    .Append(": Recommendations=" + allSmoothOutput.Recommendations.ToString())
                    .Append(": Failures=" + allSmoothOutput.Failures.ToString())
                    .Append(": Fail Smooth instance=" + smoothInstancesFailed.ToString())
                    ;

                raiseInfo(completedSmoothStats.ToString());
            }
        }

        public static ImmutableSmoothData LoadSmoothData(
            IModelLoaders modelLoadingService,
            IReadOnlyCollection<string> salesAreaNames,
            DateTimeRange smoothPeriod,
            Action<string> raiseInfo
            ) => ImmutableSmoothData.Create(
                modelLoadingService,
                salesAreaNames,
                smoothPeriod,
                raiseInfo
                );

        private void LogSmoothConfiguration(
            Guid runId,
            SmoothConfiguration smoothConfiguration,
            string rootFolder,
            DateTime processorDateTime,
            DateTimeRange runPeriod,
            DateTimeRange smoothPeriod,
            Action<string> raiseInfo,
            Action<string> raiseWarning)
        {
            var info = new StringBuilder(320);
            _ = info
                .Append("Executing Smooth ")
                .Append($"[Configuration Version {smoothConfiguration.Version} ")
                .Append($"Run Period {Log(runPeriod)} ID [{Log(runId)}], ")
                .Append($"Smooth Period {Log(smoothPeriod.Start)} to {Log(smoothPeriod.End)}]")
                ;

            raiseInfo(info.ToString());

            if (String.IsNullOrWhiteSpace(rootFolder))
            {
                raiseWarning("No folder was specified for the Smooth configuration to be saved to.");

                return;
            }

            var factory = new LogFilenameFactory(
                runId,
                rootFolder,
                processorDateTime
                );

            string filename = factory.FilenameFor(
                LogFilenameFactory.LogFileType.SmoothConfiguration);

            if (File.Exists(filename))
            {
                return;
            }

            try
            {
                File.WriteAllText(
                    filename,
                    JsonConvert.SerializeObject(
                        smoothConfiguration,
                        Formatting.Indented)
                    );
            }
            catch { }
        }

        private void LogSmoothFailureMessages(
            IDictionary<int, int> spotsByFailureMessage,
            Action<string> info)
        {
            string primaryLanguage = Globals.SupportedLanguages[0];

            foreach (var smoothFailureMessage in _modelLoaders.GetAllSmoothFailureMessages())
            {
                int spotsCountByFailureMessage = spotsByFailureMessage.ContainsKey(smoothFailureMessage.Id)
                    ? spotsByFailureMessage[smoothFailureMessage.Id]
                    : 0;

                info($"Spots with Smooth failure '{smoothFailureMessage.Description[primaryLanguage]}'={spotsCountByFailureMessage.ToString()}");
            }
        }

        private static IReadOnlyList<SmoothStatistics> PrepareSmoothStatisticsForSalesAreas(
            IReadOnlyList<SalesArea> salesAreas
            )
        {
            var result = new List<SmoothStatistics>();

            foreach (string salesAreaName in salesAreas.Select(s => s.Name))
            {
                result.Add(new SmoothStatistics(salesAreaName));
            }

            return result;
        }
    }
}
