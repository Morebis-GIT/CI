namespace xggameplan.CSVImporter
{
    public class LmkKpiIndexMap : OutputFileMap<LmkKpiImport>
    {
        public LmkKpiIndexMap()
        {
            Map(m => m.RatingCampaignsRatedSpots).Index(0);
            Map(m => m.SpotCampaignsRatedSpots).Index(1);
            Map(m => m.ZeroRatedSpotsBooked).Index(2);
            Map(m => m.TotalSpotsBooked).Index(3);
            Map(m => m.RatingCampaignsRatedSpotsIsrRemoved).Index(4);
            Map(m => m.SpotCampaignsRatedSpotsIsrRemoved).Index(5);
            Map(m => m.ZeroRatedSpotsIsrRemoved).Index(6);
            Map(m => m.TotalSpotsIsrRemoved).Index(7);
            Map(m => m.RatingCampaignsRatedSpotsRsRemoved).Index(8);
            Map(m => m.SpotCampaignsRatedSpotsIsrRemoved).Index(9);
            Map(m => m.ZeroRatedSpotsRsRemoved).Index(10);
            Map(m => m.TotalSpotsRsRemoved).Index(11);
            Map(m => m.BaseRatingsAchieved).Index(12);
            Map(m => m.BaseRatingsAchieved30).Index(13);
            Map(m => m.RatingCampaignsExistingSpots).Index(14);
            Map(m => m.SpotCampaignsExistingSpots).Index(15);
            Map(m => m.NominalValueDelivered).Index(16);
            Map(m => m.TotalNominalValue).Index(17);
            Map(m => m.PlusMinusValueDelivered).Index(18);
            Map(m => m.PercentageValueDelivered).Index(19);
            Map(m => m.PlusMinusValueDeliveredIncludingPayback).Index(20);
            Map(m => m.PercentageValueDeliveredIncludingPayback).Index(21);
            Map(m => m.TotalCampaignRevenue).Index(22);
            Map(m => m.TotalCampaignPayback).Index(23);
        }
    }
}
