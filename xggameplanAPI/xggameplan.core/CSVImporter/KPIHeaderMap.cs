namespace xggameplan.CSVImporter
{
    public class KPIHeaderMap : OutputFileMap<KPIImport>
    {
        public KPIHeaderMap()
        {
            Map(m => m.CampaignCompletionLessThen5).Name("camp_0_5");
            Map(m => m.CampaignCompletionFrom5To10).Name("camp_5_10");
            Map(m => m.CampaignCompletionFrom10To15).Name("camp_10_15");
            Map(m => m.CampaignCompletionFrom15To20).Name("camp_15_20");
            Map(m => m.CampaignCompletionFrom20To25).Name("camp_20_25");
            Map(m => m.CampaignCompletionFrom25To30).Name("camp_25_30");
            Map(m => m.CampaignCompletionFrom30To35).Name("camp_30_35");
            Map(m => m.CampaignCompletionFrom35To40).Name("camp_35_40");
            Map(m => m.CampaignCompletionFrom40To45).Name("camp_40_45");
            Map(m => m.CampaignCompletionFrom45To50).Name("camp_45_50");
            Map(m => m.CampaignCompletionFrom50To55).Name("camp_50_55");
            Map(m => m.CampaignCompletionFrom55To60).Name("camp_55_60");
            Map(m => m.CampaignCompletionFrom60To65).Name("camp_60_65");
            Map(m => m.CampaignCompletionFrom65To70).Name("camp_65_70");
            Map(m => m.CampaignCompletionFrom70To75).Name("camp_70_75");
            Map(m => m.CampaignCompletionFrom75To80).Name("camp_75_80");
            Map(m => m.CampaignCompletionFrom80To85).Name("camp_80_85");
            Map(m => m.CampaignCompletionFrom85To90).Name("camp_85_90");
            Map(m => m.CampaignCompletionFrom90To95).Name("camp_90_95");
            Map(m => m.CampaignCompletionFrom95To100).Name("camp_95_100");
            Map(m => m.CampaignCompletionFrom100To105).Name("camp_100_105");
            Map(m => m.CampaignCompletionFrom105To110).Name("camp_105_110");
            Map(m => m.CampaignCompletionFrom110To115).Name("camp_110_115");
            Map(m => m.CampaignCompletionOver115).Name("camp_over_115");
            Map(m => m.AverageEfficiency).Name("average_eff");
            Map(m => m.TotalSpotsBooked).Name("total_spots");
            Map(m => m.RemainingAudience).Name("base_rtgs_post");
            Map(m => m.RemainingAvailability).Name("total_open_avail_post");
            Map(m => m.StandardAverageCompletion).Name("standard_avg_comp");
            Map(m => m.WeightedAverageCompletion).Name("weighted_avg_comp");
        }
    }
}
