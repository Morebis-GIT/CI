using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Programmes.Objects;
using ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Dtos;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using xggameplan.common;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services
{
    /// <summary>
    /// Smooth diagnostics logged to file.
    /// </summary>
    public class FileSmoothDiagnostics : ISmoothDiagnostics
    {
        private readonly Guid _runId;
        private readonly string _salesAreaName;
        private readonly string _processorDateTimeToString;

        private readonly bool _isloggingEnabled;
        private readonly LogFilenameFactory _logFilenameFactory;

        // TODO: Make this configurable, only really enabled for debugging
        private readonly bool _logBestBreakFactor = false;

        private readonly ISmoothConfiguration _smoothConfiguration;
        private readonly LogEntryQueue<string> _logSpotActionsQueue;

        private const char Delimiter = (char)9;

        /// <summary>
        /// Log entry queue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class LogEntryQueue<T>
        {
            /// <summary>
            /// Whether queue is enabled
            /// </summary>
            public bool Enabled { get; set; }

            /// <summary>
            /// Max number of low entries stored in memory before flushing
            /// </summary>
            public int MaxLogEntries { get; set; }

            /// <summary>
            /// Log entries queued
            /// </summary>
            public List<T> LogEntries = new List<T>();
        }

        public FileSmoothDiagnostics(
            Guid runId,
            string salesAreaName,
            DateTime processorDateTime,
            string rootFolder,
            ISmoothConfiguration smoothConfiguration)
        {
            _runId = runId;
            _salesAreaName = salesAreaName;
            _processorDateTimeToString = processorDateTime.ToString(
                "dd-MM-yyyy",
                CultureInfo.InvariantCulture);

            _isloggingEnabled = !String.IsNullOrWhiteSpace(rootFolder);

            if (_isloggingEnabled)
            {
                _logFilenameFactory = new LogFilenameFactory(
                    _salesAreaName,
                    rootFolder,
                    processorDateTime
                    );
            }

            _smoothConfiguration = smoothConfiguration;

            // LogSpotAction should batch up entries for performance
            _logSpotActionsQueue = new LogEntryQueue<string>()
            {
                Enabled = true,
                MaxLogEntries = 500
            };
        }

        public bool LogDetail { get; set; } = false;

        /// <summary>
        /// Writes smooth spots to file for diagnostics
        /// </summary>
        /// <param name="prog"></param>
        /// <param name="smoothBreaks"></param>
        public void LogPlacedSmoothSpots(
            Programme prog,
            IReadOnlyCollection<SmoothBreak> smoothBreaks)
        {
            if (!_isloggingEnabled)
            {
                return;
            }

            string file = _logFilenameFactory.FilenameFor(LogFilenameFactory.LogFileType.Spots);
            bool addHeaders = !File.Exists(file);
            int attempts = 0;

            do
            {
                try
                {
                    ICollection<string> firstPositionInBreakRequests = PositionInBreakRequests.GetPositionInBreakRequestsForSamePosition(PositionInBreakRequests.First);
                    ICollection<string> lastPositionInBreakRequests = PositionInBreakRequests.GetPositionInBreakRequestsForSamePosition(PositionInBreakRequests.Last);

                    attempts++;
                    using var writer = new StreamWriter(file, true);
                    if (addHeaders)
                    {
                        WritePlacedSmoothSpotHeader(writer);
                    }

                    foreach (SmoothBreak smoothBreak in smoothBreaks)
                    {
                        int countSpotsWithFIB = 0;
                        int countSpotsWithLIB = 0;
                        int countSpotsWithPIB = 0;
                        int countSpotsWithSponsor = 0;

                        var (spotCount, averageSpotLengthInSeconds) = SpotCountAndAverageLength(smoothBreak);

                        foreach (SmoothSpot smoothSpot in smoothBreak.SmoothSpots)
                        {
                            Spot spot = smoothSpot.Spot;

                            if (spot.Sponsored)
                            {
                                countSpotsWithSponsor++;
                            }

                            if (smoothSpot.IsCurrent)
                            {
                                if (!String.IsNullOrEmpty(spot.RequestedPositioninBreak))
                                {
                                    countSpotsWithPIB++;
                                    if (firstPositionInBreakRequests.Contains(spot.RequestedPositioninBreak))
                                    {
                                        countSpotsWithFIB++;
                                    }
                                    else if (lastPositionInBreakRequests.Contains(spot.RequestedPositioninBreak))
                                    {
                                        countSpotsWithLIB++;
                                    }
                                }
                            }
                            else if (!String.IsNullOrEmpty(spot.ActualPositioninBreak))
                            {
                                countSpotsWithPIB++;
                                if (firstPositionInBreakRequests.Contains(spot.ActualPositioninBreak))
                                {
                                    countSpotsWithFIB++;
                                }
                                else if (lastPositionInBreakRequests.Contains(spot.ActualPositioninBreak))
                                {
                                    countSpotsWithLIB++;
                                }
                            }
                        }

                        foreach (SmoothSpot smoothSpot in smoothBreak.SmoothSpots)
                        {
                            Spot spot = smoothSpot.Spot;

                            long spotPriority = spot.Preemptlevel;

                            writer.WriteLine(
                                "{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{0}{13}{0}{14}{0}{15}{0}{16}{0}{17}{0}{18}{0}{19}{0}{20}{0}{21}{0}{22}{0}{23}{0}{24}{0}{25}{0}{26}{0}{27}{0}{28}{0}{29}{0}{30}{0}{31}{0}{32}{0}{33}{0}{34}",
                                Delimiter,
                                _runId,
                                _processorDateTimeToString,
                                _salesAreaName,
                                prog.Id,
                                smoothBreak.TheBreak.Id,
                                smoothBreak.TheBreak.Duration.ToTimeSpan().TotalSeconds,
                                smoothBreak.TheBreak.ScheduledDate.ToString(),
                                smoothBreak.RemainingAvailability.ToTimeSpan().TotalSeconds,
                                smoothSpot.SmoothPassSequence.ToString(),
                                smoothSpot.SmoothPassIterationSequence.ToString(),
                                0,
                                smoothSpot.BestBreakFactorGroupName,
                                spot.ExternalSpotRef,
                                spotPriority,
                                smoothBreak.Position,
                                spot.ExternalBreakNo,
                                spot.BreakRequest,
                                spot.RequestedPositioninBreak,
                                spot.Sponsored,
                                spot.ActualPositioninBreak,
                                spot.MultipartSpot,
                                spot.MultipartSpotPosition,
                                spot.MultipartSpotRef,
                                spot.StartDateTime.ToString(),
                                spot.EndDateTime.ToString(),
                                spot.SpotLength.ToTimeSpan().TotalSeconds,
                                smoothSpot.BreakPositionMovedFrom == 0 ? "" : smoothSpot.BreakPositionMovedFrom.ToString(),
                                (int)smoothBreak.TheBreak.Avail.ToTimeSpan().TotalSeconds,
                                countSpotsWithFIB,
                                countSpotsWithLIB,
                                countSpotsWithPIB,
                                countSpotsWithSponsor,
                                spotCount,
                                averageSpotLengthInSeconds);
                        }
                    }

                    writer.Flush();
                    writer.Close();

                    return;
                }
                catch (Exception exception) when (IsExceptionForFileInUse(exception) && attempts < 10)
                {
                    Task.Delay(100).Wait();
                }
            } while (attempts != -1);
        }

        private static (int countSmoothBreakSpots, int averageSpotLengthInSeconds)
        SpotCountAndAverageLength(SmoothBreak smoothBreak)
        {
            var spots = smoothBreak.SmoothSpots
                .Select(s => s.Spot)
                .ToList();

            int spotsCount = spots.Count;

            TimeSpan avgSpotLength = spotsCount > 0
                ? SpotUtilities.GetAverageSpotLength(spots)
                : TimeSpan.Zero;

            int averageSpotLengthInSeconds = (int)avgSpotLength.TotalSeconds;

            return (spotsCount, averageSpotLengthInSeconds);
        }

        private static void WritePlacedSmoothSpotHeader(StreamWriter writer)
        {
            writer.WriteLine(
                "{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{0}{13}{0}{14}{0}{15}{0}{16}{0}{17}{0}{18}{0}{19}{0}{20}{0}{21}{0}{22}{0}{23}{0}{24}{0}{25}{0}{26}{0}{27}{0}{28}{0}{29}{0}{30}{0}{31}{0}{32}{0}{33}{0}{34}",
                Delimiter,
                "RunId",
                "ProcessorDateTime",
                "SalesArea",
                "ProgID",
                "BreakID",
                "BreakLength",
                "BreakScheduledDate",
                "BreakLeft",
                "Pass",
                "PassIteration",
                "PlaceSpotsSeq",
                "BestBreakFactorGroup",
                "ExternalSpotRef",
                "SpotPriority",
                "Position",
                "ExternalBreakNo",
                "BreakRequest",
                "RequestedPositionInBreak",
                "Sponsored",
                "ActualPositionInBreak",
                "MultipartSpot",
                "MultipartSpotPosition",
                "MultipartSpotRef",
                "SpotStartDate",
                "SpotEndDate",
                "SpotLength",
                "PrevBreak",
                "BreakAvail",
                "CountFIBSpots",
                "CountLIBSpots",
                "CountPIBSpots",
                "CountSponsorSpots",
                "CountSpots",
                "AvgSpotLength");
        }

        /// <summary>
        /// Writes unplaced spots to file
        /// </summary>
        public void LogUnplacedSmoothSpots(IReadOnlyCollection<Spot> spots)
        {
            if (!_isloggingEnabled)
            {
                return;
            }

            string file = _logFilenameFactory.FilenameFor(LogFilenameFactory.LogFileType.Spots);

            bool addHeaders = !File.Exists(file);
            int attempts = 0;

            do
            {
                try
                {
                    attempts++;

                    using var writer = new StreamWriter(file, true);
                    if (addHeaders)
                    {
                        WriteUnplacedSmoothSpotHeader(writer);
                    }

                    foreach (var spot in spots)
                    {
                        writer.WriteLine(
                            "{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{0}{13}{0}{14}{0}{15}{0}{16}{0}{17}{0}{18}{0}{19}{0}{20}{0}{21}{0}{22}{0}{23}{0}{24}{0}{25}{0}{26}{0}{27}{0}{28}{0}{29}{0}{30}{0}{31}{0}{32}{0}{33}{0}{34}",
                            Delimiter,
                            _runId,
                            _processorDateTimeToString,
                            _salesAreaName,
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            spot.ExternalSpotRef,
                            spot.Preemptlevel,
                            "",
                            spot.ExternalBreakNo,
                            spot.BreakRequest,
                            spot.RequestedPositioninBreak,
                            spot.Sponsored,
                            spot.ActualPositioninBreak,
                            spot.MultipartSpot,
                            spot.MultipartSpotPosition,
                            spot.MultipartSpotRef,
                            spot.StartDateTime.ToString(),
                            spot.EndDateTime.ToString(),
                            spot.SpotLength.ToTimeSpan().TotalSeconds,
                            "",
                            "",
                            "",
                            "",
                            "",
                            "",
                            0,
                            0);
                    }

                    writer.Flush();
                    writer.Close();

                    return;
                }
                catch (Exception exception) when (IsExceptionForFileInUse(exception) && attempts < 10)
                {
                    // Wait before retry
                    Thread.Sleep(100);
                }
            } while (attempts != -1);
        }

        private static void WriteUnplacedSmoothSpotHeader(StreamWriter writer)
        {
            writer.WriteLine(
                "{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}{0}{11}{0}{12}{0}{13}{0}{14}{0}{15}{0}{16}{0}{17}{0}{18}{0}{19}{0}{20}{0}{21}{0}{22}{0}{23}{0}{24}{0}{25}{0}{26}{0}{27}{0}{28}{0}{29}{0}{30}{0}{31}{0}{32}{0}{33}{0}{34}",
                Delimiter,
                "RunId",
                "ProcessorDateTime",
                "SalesArea",
                "ProgID",
                "BreakID",
                "BreakLength",
                "BreakScheduledDate",
                "BreakLeft",
                "Pass",
                "PassIteration",
                "PlaceSpotsSeq",
                "BestBreakFactorGroup",
                "ExternalSpotRef",
                "SpotPriority",
                "Position",
                "ExternalBreakNo",
                "BreakRequest",
                "RequestedPositionInBreak",
                "Sponsored",
                "ActualPositionInBreak",
                "MultipartSpot",
                "MultipartSpotPosition",
                "MultipartSpotRef",
                "SpotStartDate",
                "SpotEndDate",
                "SpotLength",
                "PrevBreak",
                "BreakAvail",
                "CountFIBSpots",
                "CountLIBSpots",
                "CountPIBSpots",
                "CountSponsorSpots",
                "CountSpots",
                "AvgSpotLength");
        }

        public void LogProgramme(
            Guid smoothProgId,
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks,
            int countSpotsUnplacedBeforeSmooth,
            int countSpotsUnplacedAfterSmooth)
        {
            if (!_isloggingEnabled || !LogDetail)
            {
                return;
            }

            string file = _logFilenameFactory.FilenameFor(LogFilenameFactory.LogFileType.Programmes);

            bool addHeaders = !File.Exists(file);
            int attempts = 0;

            do
            {
                try
                {
                    attempts++;
                    using (var writer = new StreamWriter(file, true))
                    {
                        if (addHeaders)
                        {
                            writer.WriteLine(string.Format(
                                "{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}",
                                Delimiter,
                                "ProcessorDateTime",
                                "ProgID",
                                "CountBreaks",
                                "SpotsUnplacedBeforeSmooth",
                                "SpotsUnplacedAfterSmooth",
                                "CountSpotsPlaced"));
                        }

                        writer.WriteLine(string.Format(
                            "{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}",
                            Delimiter,
                            _processorDateTimeToString,
                            smoothProgId,
                            progSmoothBreaks.Count,
                            countSpotsUnplacedBeforeSmooth,
                            countSpotsUnplacedAfterSmooth,
                            countSpotsUnplacedBeforeSmooth - countSpotsUnplacedAfterSmooth));

                        writer.Flush();
                        writer.Close();
                        attempts = -1;
                    }
                }
                catch (Exception exception) when (IsExceptionForFileInUse(exception) && attempts < 10)
                {
                    // Wait before retry
                    Thread.Sleep(100);
                }
            } while (attempts != -1);
        }

        public void LogBestBreakFactorMessage(
            SmoothPass smoothPass,
            SmoothPassDefaultIteration smoothPassIteration,
            BestBreakFactorGroup bestBreakFactorGroup,
            Break theBreak,
            IReadOnlyCollection<Spot> spots,
            decimal? bestBreakScore,
            string message)
        {
            if (!_isloggingEnabled || !_logBestBreakFactor)
            {
                return;
            }

            string file = _logFilenameFactory.FilenameFor(LogFilenameFactory.LogFileType.BestBreak);

            bool addHeaders = !File.Exists(file);
            int attempts = 0;

            do
            {
                attempts++;

                try
                {
                    using var writer = new StreamWriter(file, true);
                    if (addHeaders)
                    {
                        writer.WriteLine(string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}", Delimiter, "ProcessorDateTime", "PassSeq", "PassIterationSeq", "PlaceSpotsSeq", "GroupName", "GroupSeq", "ExternalBreakRef", "Spots", "BestBreakScore", "Message"));
                    }

                    writer.WriteLine(string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}{8}{0}{9}{0}{10}", Delimiter, _processorDateTimeToString,
                        smoothPass == null ? "" : smoothPass.Sequence.ToString(),
                        smoothPassIteration == null ? "" : smoothPassIteration.Sequence.ToString(),
                        0,
                        bestBreakFactorGroup == null ? "" : bestBreakFactorGroup.Name,
                        bestBreakFactorGroup == null ? "" : bestBreakFactorGroup.Sequence.ToString(),
                        theBreak == null ? "" : theBreak.ExternalBreakRef,
                        spots == null || spots.Count == 0 ? "" : SpotUtilities.GetListOfSpotExternalReferences(",", spots),
                        bestBreakScore == null ? "" : bestBreakScore.Value.ToString("0.000000000000000000000000000000"),
                        message));

                    writer.Flush();
                    writer.Close();

                    return;
                }
                catch (Exception exception) when (IsExceptionForFileInUse(exception) && attempts < 10)
                {
                    Thread.Sleep(100);
                }
            } while (attempts != -1);
        }

        private static bool IsExceptionForFileInUse(Exception exception)
        {
            return exception.Message.Contains("being used by another process");
        }

        private void LogSpotActions(IReadOnlyCollection<string> logEntries)
        {
            string file = _logFilenameFactory.FilenameFor(LogFilenameFactory.LogFileType.SpotActions);

            bool addHeaders = !File.Exists(file);
            int attempts = 0;

            do
            {
                attempts++;

                try
                {
                    using var writer = new StreamWriter(file, true);
                    if (addHeaders)
                    {
                        WriteSpotActionHeader(writer);
                    }

                    foreach (var item in logEntries)
                    {
                        writer.WriteLine(item);
                    }

                    writer.Flush();
                    writer.Close();

                    return;
                }
                catch (Exception exception) when (IsExceptionForFileInUse(exception) && attempts < 10)
                {
                    Thread.Sleep(100);
                }
            } while (attempts != -1);

            static void WriteSpotActionHeader(StreamWriter writer) =>
                writer.WriteLine($"ProcessorDateTime{Delimiter}PassSeq{Delimiter}PassIterationSeq{Delimiter}ExternalSpotRef{Delimiter}ExternalBreakRef{Delimiter}Action{Delimiter}Message");
        }

        public void LogSpotAction(
            SmoothPass smoothPass,
            int smoothPassIteration,
            Spot spot,
            SmoothBreak smoothBreak,
            SmoothSpot.SmoothSpotActions action,
            string message)
        {
            bool loggingForThisSpotIsEnabled = _isloggingEnabled &&
                IsDiagnosticsEnabledForSpot(spot, _smoothConfiguration.DiagnosticConfiguration);

            if (!loggingForThisSpotIsEnabled)
            {
                return;
            }

            string logEntry = string.Format(
                "{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}",
                Delimiter,
                _processorDateTimeToString,
                smoothPass?.Sequence ?? 0,
                smoothPassIteration,
                spot.ExternalSpotRef,
                smoothBreak is null ? String.Empty : smoothBreak.TheBreak.ExternalBreakRef,
                action.ToString(),
                message);

            // Either log queue entry now or queue it for processing Queue
            if (_logSpotActionsQueue.Enabled)
            {
                // New log entry would exceed limit
                if (_logSpotActionsQueue.LogEntries.Count + 1 > _logSpotActionsQueue.MaxLogEntries)
                {
                    FlushLogSpotActions();
                }

                _logSpotActionsQueue.LogEntries.Add(logEntry);
            }
            else
            {
                LogSpotActions(new string[] { logEntry });
            }
        }

        public void Flush()
        {
            FlushLogSpotActions();
        }

        private void FlushLogSpotActions()
        {
            if (_logSpotActionsQueue.LogEntries.Count == 0)
            {
                return;
            }

            try
            {
                LogSpotActions(_logSpotActionsQueue.LogEntries);
            }
            finally
            {
                _logSpotActionsQueue.LogEntries.Clear();
            }
        }

        /// <summary>
        /// Returns whether diagnostics are enabled for the spot
        /// </summary>
        private bool IsDiagnosticsEnabledForSpot(
            Spot spot,
            SmoothDiagnosticConfiguration diagnosticConfiguration)
        {
            if (spot is null || _smoothConfiguration.DiagnosticConfiguration is null)
            {
                return false;
            }

            try
            {
                if (diagnosticConfiguration.SpotSalesAreas?.Contains(spot.SalesArea) == false)
                {
                    return false;
                }

                if (diagnosticConfiguration.SpotDemographics?.Contains(spot.Demographic) == false)
                {
                    return false;
                }

                if (diagnosticConfiguration.SpotExternalRefs?.Contains(spot.ExternalSpotRef) == false)
                {
                    return false;
                }

                if (diagnosticConfiguration.SpotExternalCampaignRefs?.Contains(spot.ExternalCampaignNumber) == false)
                {
                    return false;
                }

                if (diagnosticConfiguration.SpotMultipartSpots?.Contains(spot.MultipartSpot) == false)
                {
                    return false;
                }

                if (diagnosticConfiguration.SpotMinStartTime != null && spot.StartDateTime < diagnosticConfiguration.SpotMinStartTime.Value)
                {
                    return false;
                }

                if (diagnosticConfiguration.SpotMaxStartTime != null && spot.StartDateTime > diagnosticConfiguration.SpotMaxStartTime.Value)
                {
                    return false;
                }

                if (diagnosticConfiguration.SpotMinPreemptLevel != null && spot.Preemptlevel < diagnosticConfiguration.SpotMinPreemptLevel.Value)
                {
                    return false;
                }

                if (diagnosticConfiguration.SpotMaxPreemptLevel != null && spot.Preemptlevel > diagnosticConfiguration.SpotMaxPreemptLevel.Value)
                {
                    return false;
                }
            }
            catch { }

            return true;
        }
    }
}
