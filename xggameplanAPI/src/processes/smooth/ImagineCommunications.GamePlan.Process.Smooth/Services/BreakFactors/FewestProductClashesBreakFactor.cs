using System;
using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Products.Objects;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces.BreakFactors;
using ImagineCommunications.GamePlan.Process.Smooth.Models;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services.BreakFactors
{
    internal class FewestProductClashesBreakFactor
        : BreakFactor, IStandardScore
    {
        private readonly SmoothBreak _smoothBreak;
        private readonly IReadOnlyCollection<Spot> _spots;
        private readonly IReadOnlyDictionary<Guid, SpotInfo> _spotInfos;
        private readonly IReadOnlyDictionary<string, Product> _productsByExternalRef;
        private readonly IReadOnlyDictionary<string, Clash> _clashesByExternalRef;
        private readonly IProductClashChecker _productClashChecker;
        private readonly IClashExposureCountService _effectiveClashExposureCount;

        public FewestProductClashesBreakFactor(
            SmoothBreak smoothBreak,
            IReadOnlyCollection<Spot> spots,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            IReadOnlyDictionary<string, Product> productsByExternalRef,
            IReadOnlyDictionary<string, Clash> clashesByExternalRef,
            IProductClashChecker productClashChecker,
            IClashExposureCountService effectiveClashExposureCount)
        {
            _smoothBreak = smoothBreak;
            _spots = spots;
            _spotInfos = spotInfos;
            _productsByExternalRef = productsByExternalRef;
            _clashesByExternalRef = clashesByExternalRef;
            _productClashChecker = productClashChecker;
            _effectiveClashExposureCount = effectiveClashExposureCount;
        }

        public double StandardScore
        {
            // Lower factorScore is best score
            get
            {
                const double minStandardScore = 1;
                const double maxFactorScore = 1000;

                double factorScore = GetFactorScoreForProductClashes(
                    _smoothBreak,
                    _spots,
                    _spotInfos,
                    _productsByExternalRef,
                    _clashesByExternalRef,
                    _productClashChecker,
                    _effectiveClashExposureCount);

                factorScore = maxFactorScore - factorScore;

                return GetStandardFactorScore(
                    minStandardScore,
                    MaxStandardScore,
                    minStandardScore,
                    maxFactorScore,
                    factorScore);
            }
        }

        /// <summary>
        /// Calculates the factor score for product clashes at both child and
        /// parent level
        /// </summary>
        private double GetFactorScoreForProductClashes(
            SmoothBreak smoothBreak,
            IReadOnlyCollection<Spot> spots,
            IReadOnlyDictionary<Guid, SpotInfo> spotInfos,
            IReadOnlyDictionary<string, Product> productsByExternalRef,
            IReadOnlyDictionary<string, Clash> clashesByExternalRef,
            IProductClashChecker productClashChecker,
            IClashExposureCountService effectiveClashExposureCount)
        {
            var factorScoreCalculator = new ProductClashFactorScore(effectiveClashExposureCount);

            return factorScoreCalculator.GetFactorScoreForProductClashes(
                smoothBreak.SmoothSpots.Select(s => s.Spot).ToList(),
                spots,
                spotInfos,
                productsByExternalRef,
                clashesByExternalRef,
                productClashChecker.GetProductClashesForSingleSpot
                );
        }
    }
}
