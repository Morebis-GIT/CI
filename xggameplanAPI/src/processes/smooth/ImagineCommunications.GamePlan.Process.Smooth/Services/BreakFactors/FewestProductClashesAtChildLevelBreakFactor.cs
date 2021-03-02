using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces.BreakFactors;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using ImagineCommunications.GamePlan.Process.Smooth.Types;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services.BreakFactors
{
    internal class FewestProductClashesAtChildLevelBreakFactor
        : BreakFactor, IStandardScore
    {
        private readonly IReadOnlyCollection<Spot> _spots;
        private readonly SmoothBreak _smoothBreak;
        private readonly IReadOnlyDictionary<Guid, SpotInfo> _spotInfos;
        private readonly IProductClashChecker _productClashChecker;

        public FewestProductClashesAtChildLevelBreakFactor(
            IReadOnlyCollection<Spot> spots,
            SmoothBreak smoothBreak,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            IProductClashChecker productClashChecker)
        {
            _spots = spots;
            _smoothBreak = smoothBreak;
            _spotInfos = spotInfos;
            _productClashChecker = productClashChecker;
        }

        public double StandardScore
        {
            // Lowest clash count has highest score
            get
            {
                const double minStandardScore = 1;

                int productClashCount = _productClashChecker.GetProductClashesForMultipleSpots(
                    _spots,
                    _smoothBreak.SmoothSpots.Select(s => s.Spot).ToList(),
                    _spotInfos,
                    ClashCodeLevel.Child).Count;

                const double maxFactorScore = 1000;

                double factorScore = maxFactorScore - productClashCount;

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
