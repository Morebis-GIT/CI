using ImagineCommunications.GamePlan.Process.Smooth.Interfaces.BreakFactors;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services.BreakFactors
{
    internal class SameNonZeroScoreForAllBreaksBreakFactor
        : BreakFactor, IStandardScore
    {
        public double StandardScore
        {
            get
            {
                const double minStandardScore = 1;
                const double maxFactorScore = 1000;
                const double factorScore = 1000;

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
