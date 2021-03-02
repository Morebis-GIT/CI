using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters.Objects;

namespace ImagineCommunications.GamePlan.Domain.AutoBookApi.DefaultParameters
{
    public interface IAutoBookDefaultParameters
    {
        Guid Id { get; set; }
        AgBreak AgBreak { get; set; }
        AgCampaign AgCampaign { get; set; }
        AgCampaignSalesArea AgCampaignSalesArea { get; set; }
        AgExposure AgExposure { get; set; }
        List<AgHfssDemo> AgHfssDemos { get; set; }
        AgISRTimeBand AgISRTimeBand { get; set; }
        AgPeakStartEndTime AgPeakStartEndTime { get; set; }
        AgProgRestriction AgProgRestriction { get; set; }
        AgProgTxDetail AgProgTxDetail { get; set; }
        AgRestriction AgRestriction { get; set; }
        AgSpot AgSpot { get; set; }
    }
}
