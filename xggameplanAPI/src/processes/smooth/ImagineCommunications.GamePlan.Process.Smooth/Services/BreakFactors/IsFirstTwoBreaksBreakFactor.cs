using ImagineCommunications.GamePlan.Process.Smooth.Interfaces.BreakFactors;
using ImagineCommunications.GamePlan.Process.Smooth.Models;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services.BreakFactors
{
    // First two breaks has score, others have zero. Filter.
    internal class IsFirstTwoBreaksBreakFactor
        : BreakFactor, IStandardScore
    {
        private readonly SmoothBreak _smoothBreak;

        public IsFirstTwoBreaksBreakFactor(
            SmoothBreak smoothBreak) => _smoothBreak = smoothBreak;

        public double StandardScore
        {
            get
            {
                const double minStandardScore = 0;
                const double maxFactorScore = 1;

                double factorScore = (_smoothBreak.Position <= 2) ? 1 : 0;

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
