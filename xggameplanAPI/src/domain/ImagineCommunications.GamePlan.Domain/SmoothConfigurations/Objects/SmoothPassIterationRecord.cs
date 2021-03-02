using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.SmoothConfigurations.Objects
{
    public class SmoothPassIterationRecord
    {
        public List<int> PassSequences { get; set; }

        public SpotsCriteria SpotsCriteria { get; set; }

        public SmoothPassDefaultIteration PassDefaultIteration { get; set; }

        public SmoothPassUnplacedIteration PassUnplacedIteration { get; set; }
    }
}
