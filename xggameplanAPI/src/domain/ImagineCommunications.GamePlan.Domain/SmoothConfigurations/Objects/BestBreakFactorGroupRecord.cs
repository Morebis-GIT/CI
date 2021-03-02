using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects
{
    public class BestBreakFactorGroupRecord
    {
        public List<int> PassSequences { get; set; }

        public SpotsCriteria SpotsCriteria { get; set; }

        public BestBreakFactorGroup BestBreakFactorGroup { get; set; }
    }
}
