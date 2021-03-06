﻿using System.Collections.Generic;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Clashes
{
    public class CreateClash
    {
        public string ParentExternalidentifier { get; set; }
        public string Description { get; set; }
        public int DefaultPeakExposureCount { get; set; }
        public int DefaultOffPeakExposureCount { get; set; }
        public string Externalref { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
        public List<ClashDifference> Differences { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
    }
}
