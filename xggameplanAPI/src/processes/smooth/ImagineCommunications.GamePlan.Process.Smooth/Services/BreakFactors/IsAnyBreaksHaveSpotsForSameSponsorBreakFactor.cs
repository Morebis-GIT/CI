using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces.BreakFactors;
using ImagineCommunications.GamePlan.Process.Smooth.Models;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services.BreakFactors
{
    internal class IsAnyBreaksHaveSpotsForSameSponsorBreakFactor
        : BreakFactor, IStandardScore
    {
        private readonly List<Spot> _sponsoredSpots;
        private readonly IReadOnlyCollection<SmoothBreak> _progSmoothBreaks;
        private readonly IReadOnlyDictionary<Guid, SpotInfo> _spotInfos;

        public IsAnyBreaksHaveSpotsForSameSponsorBreakFactor(
            List<Spot> sponsoredSpots,
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos)
        {
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
                    // Check if other breaks have sponsored spots
                    bool anyBreaksHaveSponsoredSpots = false;

                    foreach (SmoothBreak currentSmoothBreak in _progSmoothBreaks)
                    {
                        var sponsorSpots = currentSmoothBreak.GetSponsorSpots(_sponsoredSpots, _spotInfos);
                        if (sponsorSpots.Count > 0)
                        {
                            anyBreaksHaveSponsoredSpots = true;
                            break;
                        }
                    }

                    if (anyBreaksHaveSponsoredSpots)
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
