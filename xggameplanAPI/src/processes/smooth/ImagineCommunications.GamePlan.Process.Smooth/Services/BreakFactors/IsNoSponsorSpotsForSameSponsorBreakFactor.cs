using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces.BreakFactors;
using ImagineCommunications.GamePlan.Process.Smooth.Models;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services.BreakFactors
{
    /// <summary>
    /// No matching sponsor spots has score, matching sponsor spots have zero. Filter.
    /// </summary>
    internal class IsNoSponsorSpotsForSameSponsorBreakFactor
        : BreakFactor, IStandardScore
    {
        private readonly SmoothBreak _smoothBreak;
        private readonly IReadOnlyCollection<Spot> _spots;
        private readonly IReadOnlyDictionary<Guid, SpotInfo> _spotInfos;

        public IsNoSponsorSpotsForSameSponsorBreakFactor(
            SmoothBreak smoothBreak,
            IReadOnlyCollection<Spot> spots,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos)
        {
            _smoothBreak = smoothBreak;
            _spots = spots;
            _spotInfos = spotInfos;
        }

        public double StandardScore
        {
            get
            {
                const double minStandardScore = 0;
                const double maxFactorScore = 1;

                double factorScore = _smoothBreak.GetSponsorSpots(_spots, _spotInfos).Count == 0 ? 1 : 0;

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
