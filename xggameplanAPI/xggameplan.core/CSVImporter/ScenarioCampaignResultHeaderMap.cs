namespace xggameplan.CSVImporter
{
    public class ScenarioCampaignResultHeaderMap : OutputFileMap<ScenarioCampaignResultImport>
    {
        public ScenarioCampaignResultHeaderMap()
        {
            Map(m => m.CampaignExternalId).Name("external_ref");
            Map(m => m.SalesAreaNo).Name("sare_no");
            Map(m => m.SpotLength).Name("sslg_length");
            Map(m => m.StrikeWeightStartDate).Name("ddpd_stt_date");
            Map(m => m.StrikeWeightEndDate).Name("ddpd_end_date");
            Map(m => m.DaypartName).Name("dcdp_name");
            Map(m => m.TargetRatings).Name("tgt_rtgs");
            Map(m => m.PreRunRatings).Name("pre_run_rtgs");
            Map(m => m.ISRCancelledRatings).Name("isr_rtgs");
            Map(m => m.ISRCancelledSpots).Name("isr_spots");
            Map(m => m.RSCancelledRatings).Name("rs_rtgs");
            Map(m => m.RSCancelledSpots).Name("rs_spots");
            Map(m => m.OptimiserRatings).Name("opt_rtgs");
            Map(m => m.OptimiserBookedSpots).Name("opt_spots");
            Map(m => m.ZeroRatedSpots).Name("opt_zero_spots");
            Map(m => m.NominalValue).Name("opt_value");
            Map(m => m.PassThatDelivered100Percent).Name("pass_100pc");
        }
    }
}
