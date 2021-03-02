using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces.BreakFactors;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services.BreakFactors
{
    internal class IsSponsoredSpotBeingPlacedWithNoBreakPositionBreakFactor
        : BreakFactor, IStandardScore
    {
        private readonly List<Spot> _sponsoredSpots;

        public IsSponsoredSpotBeingPlacedWithNoBreakPositionBreakFactor(
            List<Spot> sponsoredSpots)
        {
            _sponsoredSpots = sponsoredSpots;
        }

        public double StandardScore
        {
            // Filter
            get
            {
                const double minStandardScore = 0;
                const double maxFactorScore = 1;

                double standardScore = 0;

                if (_sponsoredSpots.Count > 0)
                {
                    // If no spot placement requests on the spot then place in
                    // the first break
                    if (_sponsoredSpots.Any(s => String.IsNullOrEmpty(s.BreakRequest)))
                    {
                        const double factorScore = 1;

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
