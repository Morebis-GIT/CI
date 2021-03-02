namespace xggameplan.CSVImporter
{
    public class ScenarioCampaignLevelResultIndexMap : OutputFileMap<ScenarioCampaignLevelResultImport>
    {
        public ScenarioCampaignLevelResultIndexMap()
        {
            Map(m => m.PeriodSubmissionNumber).Index(0);
            Map(m => m.CampaignNumber).Index(1);
            Map(m => m.CampaignExternalId).Index(2);
            Map(m => m.TargetRatings).Index(3);
            Map(m => m.PreRunRatings).Index(4);
            Map(m => m.ISRCancelledRatings).Index(5);
            Map(m => m.ISRCancelledSpots).Index(6);
            Map(m => m.RSCancelledRatings).Index(7);
            Map(m => m.RSCancelledSpots).Index(8);
            Map(m => m.OptimiserRatings).Index(9);
            Map(m => m.OptimiserBookedSpots).Index(10);
            Map(m => m.ZeroRatedSpots).Index(15);
            Map(m => m.NominalValue).Index(16);
        }
    }
}
