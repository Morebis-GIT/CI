using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces.BreakFactors;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services.BreakFactors
{
    // Filter.
    internal class IsNoSponsorSpotsForAnySponsorBreakFactor
        : BreakFactor, IStandardScore
    {
        private readonly List<Spot> _sponsoredSpots;

        public IsNoSponsorSpotsForAnySponsorBreakFactor(
            List<Spot> sponsoredSpots) => _sponsoredSpots = sponsoredSpots;

        public double StandardScore
        {
            get
            {
                const double minStandardScore = 0;
                const double maxFactorScore = 1;

                double factorScore = _sponsoredSpots.Count > 0 ? 0 : 1;

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
