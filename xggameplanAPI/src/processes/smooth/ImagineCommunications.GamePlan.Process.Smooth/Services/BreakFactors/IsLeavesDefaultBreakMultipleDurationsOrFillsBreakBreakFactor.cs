using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces.BreakFactors;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using NodaTime;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services.BreakFactors
{
    /// <summary>
    /// Breaks that leave 30 multiples or fill it have highest score
    /// </summary>
    internal class IsLeavesDefaultBreakMultipleDurationsOrFillsBreakBreakFactor
        : BreakFactor, IStandardScore
    {
        private readonly IReadOnlyCollection<Spot> _spots;
        private readonly SmoothBreak _smoothBreak;

        public IsLeavesDefaultBreakMultipleDurationsOrFillsBreakBreakFactor(
            IReadOnlyCollection<Spot> spots,
            SmoothBreak smoothBreak)
        {
            _spots = spots;
            _smoothBreak = smoothBreak;
        }

        public double StandardScore
        {
            // Filter
            get
            {
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

                double factorScore = 0;

                // Fills break
                if (remaining == Duration.Zero)
                {
                    factorScore = maxFactorScore;
                }
                // Duration remaining, check if leaves exact multiples
                else if (remaining.BclCompatibleTicks > 0)
                {
                    factorScore = remaining.ToTimeSpan().TotalSeconds % defaultMultiplesSeconds == 0 ? maxFactorScore : 0;
                }

                return GetStandardFactorScore(
                    minStandardScore,
                    MaxStandardScore,
                    minStandardScore,
                    maxFactorScore,
                    factorScore);
            }
        }
    }
}
