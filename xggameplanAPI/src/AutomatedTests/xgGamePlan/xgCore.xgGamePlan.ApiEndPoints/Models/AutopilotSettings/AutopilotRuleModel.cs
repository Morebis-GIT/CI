namespace xgCore.xgGamePlan.ApiEndPoints.Models.AutopilotSettings
{
    public class AutopilotRuleModel
    {
        public string UniqueRuleKey { get; set; }
        public string RuleType { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
    }
}
