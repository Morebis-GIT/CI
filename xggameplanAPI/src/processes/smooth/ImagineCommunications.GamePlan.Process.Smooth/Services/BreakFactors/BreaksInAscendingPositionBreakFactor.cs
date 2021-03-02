using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces.BreakFactors;
using ImagineCommunications.GamePlan.Process.Smooth.Models;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services.BreakFactors
{
    internal class BreaksInAscendingPositionBreakFactor
        : BreakFactor, IStandardScore
    {
        private readonly IReadOnlyCollection<SmoothBreak> _progSmoothBreaks;
        private readonly SmoothBreak _smoothBreak;

        public BreaksInAscendingPositionBreakFactor(
            IReadOnlyCollection<SmoothBreak> progSmoothBreaks,
            SmoothBreak smoothBreak)
        {
            _progSmoothBreaks = progSmoothBreaks;
            _smoothBreak = smoothBreak;
        }

        public double StandardScore
        {
            get
            {
                const double minStandardScore = 1;
                const double maxFactorScore = 100;

                // 1st break has highest score, last has lowest
                double factorScore = _progSmoothBreaks.Last().Position - _smoothBreak.Position + 1;

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
