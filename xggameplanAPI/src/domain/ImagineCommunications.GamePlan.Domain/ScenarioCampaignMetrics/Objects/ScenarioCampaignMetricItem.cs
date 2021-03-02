namespace ImagineCommunications.GamePlan.Domain.ScenarioCampaignMetrics.Objects
{
    public class ScenarioCampaignMetricItem
    {
        public string CampaignExternalId { get; set; }
        public int TotalSpots { get; set; }
        public int ZeroRatedSpots { get; set; }
        public double NominalValue { get; set; }
        public double TotalNominalValue { get; set; }
        public double DifferenceValueDelivered { get; set; }
        public double DifferenceValueDeliveredPercentage { get; set; }
    }
}
