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
    internal class FewestProductClashesAtParentLevelBreakFactor
        : BreakFactor, IStandardScore
    {
        private readonly IProductClashChecker _productClashChecker;
        private readonly IReadOnlyCollection<Spot> _spots;
        private readonly SmoothBreak _smoothBreak;
        private readonly IReadOnlyDictionary<Guid, SpotInfo> _spotInfos;

        public FewestProductClashesAtParentLevelBreakFactor(
            IProductClashChecker productClashChecker,
            IReadOnlyCollection<Spot> spots,
            SmoothBreak smoothBreak,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos)
        {
            _productClashChecker = productClashChecker;
            _spots = spots;
            _smoothBreak = smoothBreak;
            _spotInfos = spotInfos;
        }

        public double StandardScore
        {
            // Lowest clash count has highest score
            get
            {
                int productClashCount = _productClashChecker.GetProductClashesForMultipleSpots(
                    _spots,
                    _smoothBreak.SmoothSpots.Select(s => s.Spot).ToList(),
                    _spotInfos,
                    ClashCodeLevel.Parent).Count;

                const double minStandardScore = 1;
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
