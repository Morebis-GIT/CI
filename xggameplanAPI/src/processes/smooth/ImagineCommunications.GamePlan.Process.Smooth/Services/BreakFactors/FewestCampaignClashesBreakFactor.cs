using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces.BreakFactors;
using ImagineCommunications.GamePlan.Process.Smooth.Models;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services.BreakFactors
{
    internal class FewestCampaignClashesBreakFactor
        : BreakFactor, IStandardScore
    {
        private readonly IReadOnlyCollection<Spot> _spots;
        private readonly SmoothBreak _smoothBreak;
        private readonly ICampaignClashChecker _campaignClashChecker;

        public FewestCampaignClashesBreakFactor(
            IReadOnlyCollection<Spot> spots,
            SmoothBreak smoothBreak,
            ICampaignClashChecker campaignClashChecker)
        {
            _spots = spots;
            _smoothBreak = smoothBreak;
            _campaignClashChecker = campaignClashChecker;
        }

        public double StandardScore
        {
            // Lowest clash count has highest score
            get
            {
                int campaignClashCount = _campaignClashChecker.GetCampaignClashesForNewSpots(
                    _spots,
                    _smoothBreak.SmoothSpots.Select(s => s.Spot).ToList()).Count;

                const double minStandardScore = 1;
                const double maxFactorScore = 1000;

                double factorScore = maxFactorScore - campaignClashCount;

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
