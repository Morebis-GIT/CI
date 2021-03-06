﻿using System.Collections.Generic;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces.BreakFactors;
using ImagineCommunications.GamePlan.Process.Smooth.Models;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services.BreakFactors
{
    // Last break has score, others have zero. Filter.
    internal class IsLastBreakBreakFactor
        : BreakFactor, IStandardScore
    {
        private readonly SmoothBreak _smoothBreak;
        private readonly IReadOnlyCollection<SmoothBreak> _progSmoothBreaks;

        public IsLastBreakBreakFactor(
            SmoothBreak smoothBreak,
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks)
        {
            _smoothBreak = smoothBreak;
            _progSmoothBreaks = progSmoothBreaks;
        }

        public double StandardScore
        {
            get
            {
                const double minStandardScore = 0;
                const double maxFactorScore = 1;

                double factorScore = (_smoothBreak.Position == _progSmoothBreaks.Count) ? 1 : 0;

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
