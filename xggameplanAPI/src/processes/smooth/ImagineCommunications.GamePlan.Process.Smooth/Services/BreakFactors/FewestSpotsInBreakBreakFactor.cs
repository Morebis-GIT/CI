using System.Linq;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces.BreakFactors;
using ImagineCommunications.GamePlan.Process.Smooth.Models;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services.BreakFactors
{
    internal class FewestSpotsInBreakBreakFactor
        : BreakFactor, IStandardScore
    {
        private readonly SmoothBreak _smoothBreak;

        public FewestSpotsInBreakBreakFactor(
            SmoothBreak smoothBreak
            )
        {
            _smoothBreak = smoothBreak;
        }

        public double StandardScore
        {
            // Fewest spots has highest score
            get
            {
                const double minStandardScore = 1;
                const double maxFactorScore = 100;

                double factorScore = maxFactorScore - _smoothBreak.SmoothSpots.ToList().Count;

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
