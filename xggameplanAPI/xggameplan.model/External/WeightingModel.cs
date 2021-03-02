namespace xggameplan.Model
{
    public class WeightingModel
    {
        public int RuleId { get; set; }
        public string InternalType { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }

        public object Clone() => MemberwiseClone();
    }
}
