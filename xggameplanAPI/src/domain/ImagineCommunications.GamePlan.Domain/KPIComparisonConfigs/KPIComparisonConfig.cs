namespace ImagineCommunications.GamePlan.Domain.KPIComparisonConfigs
{
    public class KPIComparisonConfig
    {
        public string KPIName { get; set; }
        public float DiscernibleDifference { get; set; }
        public bool HigherIsBest { get; set; }
        public bool Ranked { get; set; } = true;
    }
}
