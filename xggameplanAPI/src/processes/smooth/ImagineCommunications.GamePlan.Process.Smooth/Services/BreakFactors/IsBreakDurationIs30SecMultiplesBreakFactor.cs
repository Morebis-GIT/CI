using ImagineCommunications.GamePlan.Process.Smooth.Interfaces.BreakFactors;
using ImagineCommunications.GamePlan.Process.Smooth.Models;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services.BreakFactors
{
    // Filter
    internal class IsBreakDurationIs30SecMultiplesBreakFactor
        : BreakFactor, IStandardScore
    {
        private readonly SmoothBreak _smoothBreak;

        public IsBreakDurationIs30SecMultiplesBreakFactor(
            SmoothBreak smoothBreak) => _smoothBreak = smoothBreak;

        public double StandardScore
        {
            get
            {
                const double minStandardScore = 0;
                const double maxFactorScore = 1;

                double factorScore = _smoothBreak.TheBreak.Duration.ToTimeSpan().TotalSeconds % 30 == 0 ? maxFactorScore : 0;

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
