using System.Collections.Generic;
using System.Linq;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces.BreakFactors;
using ImagineCommunications.GamePlan.Process.Smooth.Models;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services.BreakFactors
{
    internal class RandomBreakBreakFactor
        : BreakFactor, IStandardScore
    {
        private readonly IReadOnlyCollection<double> _randomScoreByBreak;
        private readonly SmoothBreak _smoothBreak;

        public RandomBreakBreakFactor(
            IReadOnlyCollection<double> randomScoreByBreak,
            SmoothBreak smoothBreak)
        {
            _randomScoreByBreak = randomScoreByBreak;
            _smoothBreak = smoothBreak;
        }

        public double StandardScore
        {
            get
            {
                const double minStandardScore = 1;
                const double maxFactorScore = 1000;

                double factorScore = _randomScoreByBreak.ToList()[_smoothBreak.Position - 1];

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
