namespace xggameplan.CSVImporter
{
    public class LmkKpiHeaderMap : OutputFileMap<LmkKpiImport>
    {
        public LmkKpiHeaderMap()
        {
            Map(m => m.RatingCampaignsRatedSpots).Name("rtgs_opt_booked_rated_spots");
            Map(m => m.SpotCampaignsRatedSpots).Name("spot_opt_booked_rated_spots");
            Map(m => m.ZeroRatedSpotsBooked).Name("zero_opt_booked_spots");
            Map(m => m.TotalSpotsBooked).Name("all_opt_booked_spots");
            Map(m => m.RatingCampaignsRatedSpotsIsrRemoved).Name("rtgs_isr_removed_rated_spots");
            Map(m => m.SpotCampaignsRatedSpotsIsrRemoved).Name("spot_isr_removed_rated_spots");
            Map(m => m.ZeroRatedSpotsIsrRemoved).Name("zero_isr_removed_spots");
            Map(m => m.TotalSpotsIsrRemoved).Name("all_isr_removed_spots");
            Map(m => m.RatingCampaignsRatedSpotsRsRemoved).Name("rtgs_rs_removed_rated_spots");
            Map(m => m.SpotCampaignsRatedSpotsIsrRemoved).Name("spot_rs_removed_rated_spots");
            Map(m => m.ZeroRatedSpotsRsRemoved).Name("zero_rs_removed_spots");
            Map(m => m.TotalSpotsRsRemoved).Name("all_rs_removed_spots");
            Map(m => m.BaseRatingsAchieved).Name("all_existing_rtgs_base");
            Map(m => m.BaseRatingsAchieved30).Name("all_existing_rtgs_base30");
            Map(m => m.RatingCampaignsExistingSpots).Name("rtgs_existing_spots");
            Map(m => m.SpotCampaignsExistingSpots).Name("spot_existing_spots");
            Map(m => m.NominalValueDelivered).Name("booked_nominal_value");
            Map(m => m.TotalNominalValue).Name("all_nominal_value");
            Map(m => m.PlusMinusValueDelivered).Name("pm_booked_value");
            Map(m => m.PercentageValueDelivered).Name("pc_booked_value");
            Map(m => m.PlusMinusValueDeliveredIncludingPayback).Name("pm_booked_value_pb");
            Map(m => m.PercentageValueDeliveredIncludingPayback).Name("pc_booked_value_pb");
            Map(m => m.TotalCampaignRevenue).Name("total_revenue");
            Map(m => m.TotalCampaignPayback).Name("total_payback");
        }
    }
}
