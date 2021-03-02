using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces.BreakFactors;
using ImagineCommunications.GamePlan.Process.Smooth.Models;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services.BreakFactors
{
    internal class IsOtherBreaksHaveSpotsForSameSponsorBreakFactor
        : BreakFactor, IStandardScore
    {
        private readonly SmoothBreak _smoothBreak;
        private readonly List<Spot> _sponsoredSpots;
        private readonly IReadOnlyCollection<SmoothBreak> _progSmoothBreaks;
        private readonly IReadOnlyDictionary<Guid, SpotInfo> _spotInfos;

        public IsOtherBreaksHaveSpotsForSameSponsorBreakFactor(
            SmoothBreak smoothBreak,
            List<Spot> sponsoredSpots,
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos)
        {
            _smoothBreak = smoothBreak;
            _sponsoredSpots = sponsoredSpots;
            _progSmoothBreaks = progSmoothBreaks;
            _spotInfos = spotInfos;
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
                    // If more than one spot for the sponsor then place in last
                    // break Check if other breaks have sponsored spots
                    bool otherBreaksHaveSponsoredSpots = false;
                    foreach (SmoothBreak currentSmoothBreak in _progSmoothBreaks.Where(b => b.Position != _smoothBreak.Position))
                    {
                        var sponsorSpots = currentSmoothBreak.GetSponsorSpots(_sponsoredSpots, _spotInfos);
                        if (sponsorSpots.Count > 0)
                        {
                            otherBreaksHaveSponsoredSpots = true;
                            break;
                        }
                    }

                    if (otherBreaksHaveSponsoredSpots)
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
