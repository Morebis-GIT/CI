using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces.BreakFactors;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using NodaTime;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services.BreakFactors
{
    internal class SpotDurationBalanceBreakFactor
        : BreakFactor, IStandardScore
    {
        private readonly IReadOnlyCollection<Spot> _spots;
        private readonly SmoothBreak _smoothBreak;

        public SpotDurationBalanceBreakFactor(
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
                const double minStandardScore = 1;

                // Get average length of spots in break
                var avgBreakSpotDuration = Duration.FromTimeSpan(
                    SpotUtilities.GetAverageSpotLength(
                        _smoothBreak.SmoothSpots
                            .Select(s => s.Spot)
                            .ToList()));

                // Get average length of spots that we're trying to place
                var avgSpotDuration = Duration.FromTimeSpan(
                    SpotUtilities.GetAverageSpotLength(_spots));

                // Get the difference between both averages, ensure it is a
                // positive value
                var spotDurationDifference = Duration.FromTicks(
                    Math.Abs(avgBreakSpotDuration.Minus(avgSpotDuration).BclCompatibleTicks));

                // Biggest difference has highest score, add 1 so that min score
                // is 1.
                double factorScore = (int)spotDurationDifference
                    .ToTimeSpan().TotalSeconds + 1;

                return GetStandardFactorScore(
                    minStandardScore,
                    MaxStandardScore,
                    minStandardScore,
                    MaxStandardScore,
                    factorScore);
            }
        }
    }
}
