namespace xgCore.xgGamePlan.ApiEndPoints.Models.Passes
{
    public class ToleranceModel
    {
        public int RuleId { get; set; }
        public string InternalType { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public int Under { get; set; }
        public int Over { get; set; }
        public bool Ignore { get; set; }
        public ForceOverUnder ForceOverUnder { get; set; }
        public string Type { get; set; }
    }
}
