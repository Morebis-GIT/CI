using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using ImagineCommunications.GamePlan.Domain.Breaks;
using ImagineCommunications.GamePlan.Domain.Breaks.Objects;
using Microsoft.Extensions.Logging;
using NodaTime;
using xggameplan.common.Extensions;
using xggameplan.common.Helpers;

namespace xggameplan.core.RunManagement.BreakAvailabilityCalculator
{
    /// <summary>
    /// Calculates the availability and optimiser availability for a programme's breaks, taking
    /// into account unplaced spots.
    /// </summary>
    public class BreakAndOptimiserAvailabilityCalculator<TBreak>
        where TBreak : class, IBreakAvailability
    {
        private readonly ILogger _logger;
        private readonly IBreakAvailabilityUpdateHandler<TBreak> _updateBreakHandler;

        public BreakAndOptimiserAvailabilityCalculator(ILogger logger,
            IBreakAvailabilityUpdateHandler<TBreak> updateBreakHandler)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _updateBreakHandler = updateBreakHandler ?? throw new ArgumentNullException(nameof(updateBreakHandler));
        }

        /// <summary>
        /// Calculates the availability and optimiser availability for a programme's breaks, taking
        /// into account unplaced spots.
        /// </summary>
        /// <returns>Returns the number of breaks with reduced optimiser availability.</returns>
        public void Calculate(
            string salesAreaName,
            IReadOnlyCollection<IProgrammeForBreakAvailCalculation> programmesForUtcDate,
            IReadOnlyCollection<TBreak> breaksForProgrammes,
            IReadOnlyCollection<ISpotForBreakAvailCalculation> spotsForBreaks)
        {
            foreach (var programme in programmesForUtcDate)
            {
                var programmeBreaks = breaksForProgrammes
                    .Where(theBreak => programme.DateTimeIsInProgramme(theBreak.ScheduledDate))
                    .ToList();

                if (programmeBreaks.Count == 0)
                {
                    _logger.LogInformation($"No breaks found for programme {LogAsString.Log(programme.ProgrammeId)}");
                    continue;
                }

                var breakExternalRefs = programmeBreaks.ReducePropertyToCsv(x => x.ExternalBreakRef);

                _logger.LogInformation(
                    $"Found {LogAsString.Log(programmeBreaks.Count)} breaks for "
                    + $"programme {LogAsString.Log(programme.ProgrammeId)} "
                    + $"[Ext. Break Refs: {breakExternalRefs}]");

                IReadOnlyDictionary<Guid, Duration> programmeBreakAvailabilities =
                    CalculateProgrammeBreakAvailabilities(
                        programmeBreaks,
                        spotsForBreaks
                    );

                (int unplacedSpotsCount, Duration unplacedSpotsTotalDuration) =
                    GatherProgrammeUnplacedSpotsDuration(programme, spotsForBreaks);

                _logger.LogInformation(
                    $"Found {LogAsString.Log(unplacedSpotsCount)} unplaced spot(s) "
                    + $"with total duration of {LogAsString.Log(unplacedSpotsTotalDuration)}s "
                    + $"for programme {LogAsString.Log(programme.ProgrammeId)}"
                );

                UpdateBreakAvailability(programmeBreaks, programmeBreakAvailabilities);

                if (unplacedSpotsTotalDuration == Duration.Zero)
                {
                    continue;
                }

                var availByBreak =
                    CalculateOptimiserAvailabilityForUnplacedSpots(programmeBreaks, unplacedSpotsTotalDuration);

                var countOfBreaksWithOptimizerAvailReducedForUnplacedSpots =
                    UpdateBreakOptimiserAvailability(programmeBreaks, availByBreak);

                if (countOfBreaksWithOptimizerAvailReducedForUnplacedSpots == 0)
                {
                    Duration breaksAvail = SumOfBreakAvailability(programmeBreaks);

                    int programmeBreaksCount = programmeBreaks.Count;

                    var warningMessage = new StringBuilder(256);
                    warningMessage.Append($"There were {LogAsString.Log(unplacedSpotsCount)} unplaced spots ");
                    warningMessage.Append($"for the programme {programme.ExternalReference} ");
                    warningMessage.Append($"on {LogAsString.Log(programme.StartDateTime)} for sales area {salesAreaName}. ");
                    warningMessage.Append("Unable to reduce the break optimizer availability ");
                    warningMessage.Append($"for any of the {LogAsString.Log(programmeBreaksCount)} breaks. ");
                    warningMessage.Append($"Ext. Break Refs: {breakExternalRefs}; ");
                    warningMessage.Append($"Total break availability: {LogAsString.Log(breaksAvail)}s; ");
                    warningMessage.Append($"Unplaced spots duration: {LogAsString.Log(unplacedSpotsTotalDuration)}s)");

                    _logger.LogWarning(warningMessage.ToString());
                }
            }
        }

        private Dictionary<Guid, Duration> CalculateProgrammeBreakAvailabilities(
            IReadOnlyCollection<TBreak> programmeBreaks,
            IReadOnlyCollection<ISpotForBreakAvailCalculation> spots)
        {
            var programmeBreakAvailabilities = new Dictionary<Guid, Duration>();

            foreach (var theBreak in programmeBreaks)
            {
                var bookedSpotDuration = spots
                    .Where(spot => spot.ExternalBreakNo == theBreak.ExternalBreakRef)
                    .Select(spot => spot.SpotLength)
                    .Aggregate(Duration.Zero, (current, next) => current.Plus(next));

                var breakAvailability = theBreak.Duration.Minus(bookedSpotDuration);

                _logger.LogInformation(
                    $"[Ext. Break Ref {theBreak.ExternalBreakRef}] " +
                    $"Duration {LogAsString.Log(theBreak.Duration)}s; " +
                    $"calculated availability {LogAsString.Log(breakAvailability)}s"
                );

                programmeBreakAvailabilities.Add(theBreak.Id, breakAvailability);
            }

            return programmeBreakAvailabilities;
        }

        private (int count, Duration duration) GatherProgrammeUnplacedSpotsDuration(
            IProgrammeForBreakAvailCalculation programme,
            IEnumerable<ISpotForBreakAvailCalculation> spots)
        {
            int count = 0;
            Duration unplacedSpotDuration = Duration.Zero;

            foreach (var spot in spots.Where(spot => programme.DateTimeIsInProgramme(spot.StartDateTime) && !spot.IsBooked))
            {
                ++count;
                unplacedSpotDuration = unplacedSpotDuration.Plus(spot.SpotLength);
            }

            return (count, unplacedSpotDuration);
        }

        /// <summary>
        /// Calculate the optimiser availability for unplaced spots.
        /// </summary>
        /// <param name="programmeBreaks">A list of programme breaks.</param>
        /// <param name="unplacedSpotsDuration">Total duration of unplaced spots. The amount that we
        /// need to adjust the breaks by.</param>
        /// <returns></returns>
        private IReadOnlyList<Duration> CalculateOptimiserAvailabilityForUnplacedSpots(
            IEnumerable<TBreak> programmeBreaks,
            Duration unplacedSpotsDuration
            )
        {
            var availByBreak = CurrentBreakAvailabilities(programmeBreaks);
            Duration unplacedSpotsLengthRemaining = unplacedSpotsDuration;

            // Check breaks and see if we can reduce availability
            bool done = false;
            while (!done)
            {
                if (AllUnplacedSpotDurationReallocated(unplacedSpotsLengthRemaining))
                {
                    _logger.LogInformation("All unplaced spot duration reallocated");

                    done = true;
                    continue;
                }

                Duration breaksAvail = CurrentTotalBreakAvailability(availByBreak);
                if (NoBreakAvailability(breaksAvail))
                {
                    _logger.LogInformation(
                        "No more break availability. " +
                        $"Remaining unplaced spot duration: {LogAsString.Log(unplacedSpotsLengthRemaining)}s"
                        );

                    done = true;
                    continue;
                }

                int numberOfBreaksModified = 0;

                for (int breakIndex = 0; breakIndex < availByBreak.Count; breakIndex++)
                {
                    if (availByBreak[breakIndex] <= Duration.Zero)
                    {
                        continue;
                    }

                    var availToReduceBreakBy = Duration.FromSeconds(15);
                    if (unplacedSpotsLengthRemaining < availToReduceBreakBy)
                    {
                        availToReduceBreakBy = unplacedSpotsLengthRemaining;
                    }

                    if (availByBreak[breakIndex] >= availToReduceBreakBy)
                    {
                        availByBreak[breakIndex] = availByBreak[breakIndex].Minus(availToReduceBreakBy);
                        unplacedSpotsLengthRemaining = unplacedSpotsLengthRemaining.Minus(availToReduceBreakBy);

                        numberOfBreaksModified++;
                    }

                    if (unplacedSpotsLengthRemaining <= Duration.Zero)
                    {
                        done = true;
                        break;
                    }
                }

                if (numberOfBreaksModified == 0)
                {
                    _logger.LogInformation("No breaks modified");
                    done = true;
                }
            }

            return availByBreak;
        }

        [Pure]
        private static Duration CurrentTotalBreakAvailability(IEnumerable<Duration> durations) =>
            durations.Aggregate(Duration.Zero, (current, next) => current + next);

        [Pure]
        private static Duration SumOfBreakAvailability(IEnumerable<TBreak> breaks) =>
            breaks.Select(x => x.Avail).Aggregate(Duration.Zero, (current, next) => current + next);

        /// <summary>
        /// Calculate the current availabilities for the programme's breaks.
        /// </summary>
        /// <param name="breaks">A list of <see cref="Break"/> instances for a Programme.</param>
        [Pure]
        private static List<Duration> CurrentBreakAvailabilities(IEnumerable<TBreak> breaks) =>
            breaks.Select(x => x.Avail).ToList();

        [Pure]
        private static string LogPrologue(string breakExternalReference) =>
            $"[Ext. Break Ref {breakExternalReference}] ";

        private int UpdateBreakOptimiserAvailability(
            IReadOnlyCollection<TBreak> programmeBreaks,
            IReadOnlyList<Duration> availByBreak)
        {
            int countOfBreaksWithOptimizerAvailReducedForUnplacedSpots = 0;
            int idx = 0;

            foreach (var theBreak in programmeBreaks)
            {
                Duration GetBreakOptimiserAvailability() => availByBreak[idx++];

                Duration breakOptimiserAvailability = GetBreakOptimiserAvailability();

                if (UpdateOnlyBreakOptimiserAvailIfChanged(theBreak, breakOptimiserAvailability))
                {
                    countOfBreaksWithOptimizerAvailReducedForUnplacedSpots++;
                    _updateBreakHandler.UpdateOptimizerAvailability(theBreak);
                }
            }

            return countOfBreaksWithOptimizerAvailReducedForUnplacedSpots;
        }

        private void UpdateBreakAvailability(
            IReadOnlyCollection<TBreak> programmeBreaks,
            IReadOnlyDictionary<Guid, Duration> programmeBreakDurations)
        {
            foreach (var theBreak in programmeBreaks)
            {
                if (!programmeBreakDurations.TryGetValue(theBreak.Id, out var breakAvailability))
                {
                    breakAvailability = Duration.Zero;
                }

                if (UpdateBreakAvailAndOptimiserAvailIfChanged(theBreak, breakAvailability))
                {
                    _updateBreakHandler.UpdateAvailability(theBreak);
                }
            }
        }

        private bool UpdateBreakAvailAndOptimiserAvailIfChanged(TBreak theBreak, Duration breakAvailability)
        {
            var result = false;
            var infoMessage = new StringBuilder(128);
            infoMessage
                .Append(LogPrologue(theBreak.ExternalBreakRef))
                .Append("Avail: ").Append(LogAsString.Log(theBreak.Avail)).Append("s; ")
                .Append("OptimiserAvail: ").Append(LogAsString.Log(theBreak.OptimizerAvail)).Append('s');

            if (theBreak.HasAvailabilityChanged(breakAvailability))
            {
                theBreak.Avail = breakAvailability;
                theBreak.OptimizerAvail = breakAvailability;

                infoMessage
                    .Append("; Both now ")
                    .Append(LogAsString.Log(breakAvailability))
                    .Append('s');
                result = true;
            }

            _logger.LogInformation(infoMessage.ToString());

            return result;
        }

        /// <summary>
        /// Update the break's optimiser availability if the value has changed.
        /// </summary>
        /// <returns>Returns true if the break was updated. Also returns an information
        /// message.</returns>
        private bool UpdateOnlyBreakOptimiserAvailIfChanged(TBreak theBreak, Duration breakOptimiserAvailability)
        {
            var result = false;
            var infoMessage = new StringBuilder(128);
            infoMessage
                .Append(LogPrologue(theBreak.ExternalBreakRef))
                .Append("Avail: ").Append(LogAsString.Log(theBreak.Avail)).Append("s; ")
                .Append("OptimiserAvail: ").Append(LogAsString.Log(theBreak.OptimizerAvail)).Append('s');

            if (theBreak.HasAvailabilityChanged(breakOptimiserAvailability))
            {
                theBreak.OptimizerAvail = breakOptimiserAvailability;

                infoMessage
                    .Append("; OptimiserAvail now ")
                    .Append(LogAsString.Log(breakOptimiserAvailability))
                    .Append('s');
                result = true;
            }

            _logger.LogInformation(infoMessage.ToString());

            return result;
        }

        [Pure]
        private static bool AllUnplacedSpotDurationReallocated(Duration remainingDuration) =>
            remainingDuration <= Duration.Zero;

        [Pure]
        private static bool NoBreakAvailability(Duration breaksAvail) =>
            breaksAvail <= Duration.Zero;
    }
}
