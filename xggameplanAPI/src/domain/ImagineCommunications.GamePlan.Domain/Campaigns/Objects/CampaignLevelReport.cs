using ImagineCommunications.GamePlan.Domain.Generic.Interfaces;

namespace ImagineCommunications.GamePlan.Domain.Campaigns.Objects
{
    public class CampaignLevelReport : ICampaignKpiData
    {
        public double NominalValue { get; set; }
        public double? Payback { get; set; }
        public double? RevenueBudget { get; set; }
    }
}
