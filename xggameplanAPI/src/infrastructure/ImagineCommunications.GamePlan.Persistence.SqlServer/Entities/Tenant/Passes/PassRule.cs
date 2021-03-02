namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes
{
    public class PassRule : PassRuleBase
    {
        public bool Ignore { get; set; }
        public string PeakValue { get; set; }
        public CampaignDeliveryType CampaignType { get; set; }
    }
}
