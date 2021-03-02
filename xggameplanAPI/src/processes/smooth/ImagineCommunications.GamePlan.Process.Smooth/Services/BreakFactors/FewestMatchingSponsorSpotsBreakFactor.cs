using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces.BreakFactors;
using ImagineCommunications.GamePlan.Process.Smooth.Models;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services.BreakFactors
{
    /// <summary>
    /// Break with fewest sponsor spots has highest score.
    /// </summary>
    internal class FewestMatchingSponsorSpotsBreakFactor
        : BreakFactor, IStandardScore
    {
        private readonly IReadOnlyCollection<Spot> _spots;
        private readonly SmoothBreak _smoothBreak;
        private readonly IReadOnlyDictionary<Guid, SpotInfo> _spotInfos;

        public FewestMatchingSponsorSpotsBreakFactor(
            IReadOnlyCollection<Spot> spots,
            SmoothBreak smoothBreak,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos)
        {
            _spots = spots;
            _smoothBreak = smoothBreak;
            _spotInfos = spotInfos;
        }

        public double StandardScore
        {
            get
            {
                const double minStandardScore = 1;
                const double maxFactorScore = 1;

                double standardScore = 0;
                double factorScore;

                foreach (var spot in _spots)
                {
                    factorScore = maxFactorScore - _smoothBreak.GetSponsorSpots(_spots, _spotInfos).Count;

                    standardScore += GetStandardFactorScore(
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
