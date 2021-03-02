using System;

namespace ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects
{
    public class BestBreakFactor
    {
        /// <summary>
        /// Priority for importance of this factor, necessary if we're
        /// evaluating multiple factors simultaneously.
        /// </summary>
        public int Priority { get; set; }

        public BestBreakFactors Factor { get; set; }

        public BestBreakFactor(int priority, BestBreakFactors factor)
        {
            // Limit based on BestBreakEvaluator score limits
            if (priority < 1 || priority > BestBreakFactorGroupItem.MaxBreakFactorPriority)
            {
                throw new ArgumentOutOfRangeException(nameof(priority));
            }

            Priority = priority;
            Factor = factor;
        }
    }
}
