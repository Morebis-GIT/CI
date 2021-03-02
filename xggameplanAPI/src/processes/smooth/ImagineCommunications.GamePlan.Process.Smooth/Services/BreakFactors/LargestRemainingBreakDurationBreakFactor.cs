using ImagineCommunications.GamePlan.Process.Smooth.Interfaces.BreakFactors;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using NodaTime;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services.BreakFactors
{
    /// <summary>
    /// Break with largest availability has highest score.
    /// </summary>
    internal class LargestRemainingBreakDurationBreakFactor
        : BreakFactor, IStandardScore
    {
        private readonly SmoothBreak _smoothBreak;

        public LargestRemainingBreakDurationBreakFactor(
            SmoothBreak smoothBreak) => _smoothBreak = smoothBreak;

        public double StandardScore
        {
            get
            {
                bool breakHasNegativeRemainingDuration = _smoothBreak.RemainingAvailability < Duration.Zero;

                const double minStandardScore = 1;
                const double maxFactorScore = 1000;

                // Add 1 to ensure that min is 1
                double factorScore = (
                    breakHasNegativeRemainingDuration
                        ? 0
                        : _smoothBreak.RemainingAvailability.ToTimeSpan().TotalSeconds
                    ) + 1;

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
