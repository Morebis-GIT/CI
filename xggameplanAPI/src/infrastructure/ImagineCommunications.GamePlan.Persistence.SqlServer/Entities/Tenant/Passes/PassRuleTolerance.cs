namespace ImagineCommunications.GamePlan.Persistence.SqlServer.Entities.Tenant.Passes
{
    public class PassRuleTolerance : PassRuleBase
    {
        public bool Ignore { get; set; }
        public int Under { get; set; }
        public int Over { get; set; }
        public ForceOverUnder ForceOverUnder { get; set; }
        public bool BookTargetArea { get; set; }
        public CampaignDeliveryType CampaignType { get; set; }
    }
}
