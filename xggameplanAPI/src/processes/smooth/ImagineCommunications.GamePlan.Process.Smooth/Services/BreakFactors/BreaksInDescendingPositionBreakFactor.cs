using System.Collections.Generic;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces.BreakFactors;
using ImagineCommunications.GamePlan.Process.Smooth.Models;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services.BreakFactors
{
    internal class BreaksInDescendingPositionBreakFactor
        : BreakFactor, IStandardScore
    {
        private readonly IReadOnlyCollection<SmoothBreak> _progSmoothBreaks;
        private readonly SmoothBreak _smoothBreak;

        public BreaksInDescendingPositionBreakFactor(
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks,
            SmoothBreak smoothBreak)
        {
            _progSmoothBreaks = progSmoothBreaks;
            _smoothBreak = smoothBreak;
        }

        public double StandardScore
        {
            // Last break has highest score, first has lowest
            get
            {
                const double minStandardScore = 1;

                double maxFactorScore = _progSmoothBreaks.Count + 1;
                double factorScore = _smoothBreak.Position;

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
