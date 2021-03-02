namespace xggameplan.CSVImporter
{
    public class LmkKpiImport
    {
        public long RatingCampaignsRatedSpots { get; set; }
        public long SpotCampaignsRatedSpots { get; set; }
        public long ZeroRatedSpotsBooked { get; set; }
        public long TotalSpotsBooked { get; set; }
        public long RatingCampaignsRatedSpotsIsrRemoved { get; set; }
        public long SpotCampaignsRatedSpotsIsrRemoved { get; set; }
        public long ZeroRatedSpotsIsrRemoved { get; set; }
        public long TotalSpotsIsrRemoved { get; set; }
        public long RatingCampaignsRatedSpotsRsRemoved { get; set; }
        public long SpotCampaignsRatedSpotsRsRemoved { get; set; }
        public long ZeroRatedSpotsRsRemoved { get; set; }
        public long TotalSpotsRsRemoved { get; set; }
        public double BaseRatingsAchieved { get; set; }
        public double BaseRatingsAchieved30 { get; set; }
        public long RatingCampaignsExistingSpots { get; set; }
        public long SpotCampaignsExistingSpots { get; set; }
        public double NominalValueDelivered { get; set; }
        public double TotalNominalValue { get; set; }
        public double PlusMinusValueDelivered { get; set; }
        public double PercentageValueDelivered { get; set; }
        public double PlusMinusValueDeliveredIncludingPayback { get; set; }
        public double PercentageValueDeliveredIncludingPayback { get; set; }
        public double TotalCampaignRevenue { get; set; }
        public double TotalCampaignPayback { get; set; }
    }
}
