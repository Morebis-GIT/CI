using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Spots;
using ImagineCommunications.GamePlan.Process.Smooth.Interfaces.BreakFactors;
using ImagineCommunications.GamePlan.Process.Smooth.Models;
using NodaTime;

namespace ImagineCommunications.GamePlan.Process.Smooth.Services.BreakFactors
{
    internal class IsFillsBreakDurationBreakFactor
        : BreakFactor, IStandardScore
    {
        private readonly SmoothBreak _smoothBreak;
        private readonly IReadOnlyCollection<Spot> _spots;

        public IsFillsBreakDurationBreakFactor(
            SmoothBreak smoothBreak,
            IReadOnlyCollection<Spot> spots)
        {
            _smoothBreak = smoothBreak;
            _spots = spots;
        }

        public double StandardScore
        {
            // Filter
            get
            {
                const double minStandardScore = 0;
                const double maxFactorScore = 1;

                var spotsLength = new Duration();

                foreach (var s in _spots)
                {
                    spotsLength = spotsLength.Plus(s.SpotLength);
                }

                double standardScore = 0;

                if (_smoothBreak.RemainingAvailability.Minus(spotsLength) == Duration.Zero)
                {
                    const double factorScore = maxFactorScore;

                    standardScore = GetStandardFactorScore(
                        minStandardScore,
                        MaxStandardScore,
                        minStandardScore,
                        maxFactorScore,
                        factorScore);
                }

                return standardScore;
            }
        }
    }
}
