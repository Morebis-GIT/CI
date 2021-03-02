using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;

namespace xggameplan.Model
{
    public class CreateClash
    {
        public string ParentExternalidentifier { get; set; }
        public string Description { get; set; }
        public int DefaultPeakExposureCount { get; set; }
        public int DefaultOffPeakExposureCount { get; set; }
        public List<ClashDifference> Differences { get; set; }

        public string Externalref { get; set; }

    }

}
