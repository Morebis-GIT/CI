using System.Diagnostics.Contracts;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services.BreakFactors
{
    internal abstract class BreakFactor
    {
        protected static double MaxStandardScore => 1_000;

        /// <summary>
        /// Calculate the standard factor score from the actual factor score so
        /// that it is possible to compare the scores for 2 factors.
        /// </summary>
        [Pure]
        internal static double GetStandardFactorScore(
            double minStandardScore,
            double maxStandardScore,
            double minFactorScore,
            double maxFactorScore,
            double factorScore
            )
        {
            if (factorScore == minFactorScore)
            {
                return minStandardScore;
            }
            else if (factorScore == maxFactorScore)
            {
                return maxStandardScore;
            }
            else
            {
                return maxStandardScore * (factorScore / maxFactorScore);
            }
        }
    }
}
