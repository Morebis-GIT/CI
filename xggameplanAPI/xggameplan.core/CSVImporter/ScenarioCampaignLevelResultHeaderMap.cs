namespace xggameplan.CSVImporter
{
    public class ScenarioCampaignLevelResultHeaderMap : OutputFileMap<ScenarioCampaignLevelResultImport>
    {
        public ScenarioCampaignLevelResultHeaderMap()
        {
            Map(m => m.PeriodSubmissionNumber).Name("aper_no");
            Map(m => m.CampaignNumber).Name("camp_no");
            Map(m => m.CampaignExternalId).Name("external_ref");
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
        }
    }
}
