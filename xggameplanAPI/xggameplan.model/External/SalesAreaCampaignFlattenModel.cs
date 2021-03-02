namespace xggameplan.Model
{
    public class SalesAreaCampaignTargetFlattenModel
    {
        public string SalesAreaGroupName { get; set; }
        public decimal SalesAreaGroupTargetRatings { get; set; }
        public decimal SalesAreaGroupActualRatings { get; set; }
        public decimal SalesAreaGroupTargetActualDiff { get; set; }
        public decimal SalesAreaGroupTargetAchievedPct { get; set; }
    }
}
