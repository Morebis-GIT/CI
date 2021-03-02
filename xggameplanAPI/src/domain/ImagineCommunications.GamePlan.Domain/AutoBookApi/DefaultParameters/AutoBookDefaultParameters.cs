using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters
{
    public class AutoBookDefaultParameters : IAutoBookDefaultParameters
    {
        public Guid Id { get; set; }

        public AgBreak AgBreak { get; set; }

        public AgCampaign AgCampaign { get; set; }

        public AgCampaignSalesArea AgCampaignSalesArea { get; set; }

        public AgExposure AgExposure { get; set; }

        public List<AgHfssDemo> AgHfssDemos { get; set; }

        public AgISRTimeBand AgISRTimeBand { get; set; }

        public AgPeakStartEndTime AgPeakStartEndTime { get; set; }

        public AgProgRestriction AgProgRestriction { get; set; }

        public AgProgTxDetail AgProgTxDetail { get; set; }

        public AgRestriction AgRestriction { get; set; }

        public AgSpot AgSpot { get; set; }
    }
}
