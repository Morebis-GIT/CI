using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces.BreakFactors;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using NodaTime;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services.BreakFactors
{
    internal class IsLeaves15SecBreakMultipleDurationsBreakFactor
        : BreakFactor, IStandardScore
    {
        private readonly IReadOnlyCollection<Spot> _spots;
        private readonly SmoothBreak _smoothBreak;

        public IsLeaves15SecBreakMultipleDurationsBreakFactor(
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
                Duration remaining = _smoothBreak.RemainingAvailability.Minus(spotsDuration);

                double standardScore = 0;

                // No remaining time should score zero
                if (_smoothBreak.RemainingAvailability > Duration.Zero && remaining > Duration.Zero)
                {
                    // Exact multiples of 15 secs remaining
                    if (remaining.ToTimeSpan().TotalSeconds % 15 == 0)
                    {
                        const double factorScore = maxFactorScore;

                        standardScore = GetStandardFactorScore(
                            minStandardScore,
                            MaxStandardScore,
                            minStandardScore,
                            maxFactorScore,
                            factorScore);
                    }
                }

                return standardScore;
            }
        }
    }
}
