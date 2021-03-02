using System.Collections.Generic;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces.BreakFactors;
using ImagineCommunications.GamePlan.Process.Smooth.Models;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services.BreakFactors
{
    // Last half of prog has score, first half has zero. Filter.
    internal class IsLastHalfOfProgrammeBreakFactor
        : BreakFactor, IStandardScore
    {
        private readonly SmoothBreak _smoothBreak;
        private readonly IReadOnlyCollection<SmoothBreak> _progSmoothBreaks;

        public IsLastHalfOfProgrammeBreakFactor(
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

                double factorScore = (_smoothBreak.Position > (_progSmoothBreaks.Count / 2)) ? 1 : 0;

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
