using System.ComponentModel;

namespace xgCore.xgGamePlan.ApiEndPoints.Models.Campaigns
{
    public enum IncludeRightSizer
    {
        [Description("No")]
        No = 0,
        [Description("Campaign Level")]
        CampaignLevel = 1,
        [Description("Detail Level")]
        DetailLevel = 2
    }
}
