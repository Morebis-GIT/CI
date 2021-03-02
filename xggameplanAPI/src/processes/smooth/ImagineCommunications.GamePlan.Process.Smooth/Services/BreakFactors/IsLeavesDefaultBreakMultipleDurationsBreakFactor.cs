using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces.BreakFactors;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using NodaTime;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services.BreakFactors
{
    internal class IsLeavesDefaultBreakMultipleDurationsBreakFactor
        : BreakFactor, IStandardScore
    {
        private readonly IReadOnlyCollection<Spot> _spots;
        private readonly SmoothBreak _smoothBreak;

        public IsLeavesDefaultBreakMultipleDurationsBreakFactor(
            IReadOnlyCollection<Spot> spots,
            SmoothBreak smoothBreak)
        {
            _spots = spots;
            _smoothBreak = smoothBreak;
        }

        public double StandardScore
        {
            get
            {
                // Filter. Breaks that leave 30 multiples have highest score.

                // If break duration is multiples of 30 secs then return 1 if
                // remaining duration leaves multiples of 30 secs.

                // If break duration is multiples of 15 secs then return 1 if
                // remaining duration leaves multiples of 15 secs.

                // If break duration is multiples of 10 secs then return 1 if
                // remaining duration leaves multiples of 10 secs.

                // TODO: Revisit this

                const double minStandardScore = 0;
                const double maxFactorScore = 1;

                var spotsDuration = Duration.FromSeconds((int)SpotUtilities.GetTotalSpotLength(_spots).TotalSeconds);

                int defaultMultiplesSeconds = 30;
                int breakSeconds = (int)_smoothBreak.TheBreak.Duration.ToTimeSpan().TotalSeconds;

                foreach (int multiple in new int[] { 30, 15, 10 })
                {
                    if (breakSeconds % multiple == 0)
                    {
                        defaultMultiplesSeconds = multiple;
                        break;
                    }
                }

                Duration remaining = _smoothBreak.RemainingAvailability.Minus(spotsDuration);

                double standardScore = 0;

                if (remaining > Duration.Zero)
                {
                    // No remaining time should score zero
                    double factorScore = remaining.ToTimeSpan().TotalSeconds % defaultMultiplesSeconds == 0 ? maxFactorScore : 0;

                    standardScore = GetStandardFactorScore(
                        minStandardScore,
                        MaxStandardScore,
                        minStandardScore,
                        maxFactorScore,
                        factorScore);
                }

                return standardScore;
            }
        }
    }
}
