using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;

namespace xggameplan.Model
{
    public class ClashModel
    {
        public Guid Uid { get; set; }
        public string Externalref { get; set; }
        public string ParentExternalidentifier { get; set; }
        public string Description { get; set; }
        public int DefaultPeakExposureCount { get; set; }
        public int DefaultOffPeakExposureCount { get; set; }
        public string ParentClashDescription { get; set; }
        public int? ParentPeakExposureCount { get; set; }
        public int? ParentOffPeakExposureCount { get; set; }
        public List<ClashDifference> Differences { get; set; }
    }
}
