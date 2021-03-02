namespace xggameplan.CSVImporter
{
    public class ScenarioCampaignResultIndexMap : OutputFileMap<ScenarioCampaignResultImport>
    {
        public ScenarioCampaignResultIndexMap()
        {
            Map(m => m.CampaignExternalId).Index(1);
            Map(m => m.SalesAreaNo).Index(3);
            Map(m => m.SpotLength).Index(5);
            Map(m => m.StrikeWeightStartDate).Index(6);
            Map(m => m.StrikeWeightEndDate).Index(7);
            Map(m => m.DaypartName).Index(8);
            Map(m => m.TargetRatings).Index(9);
            Map(m => m.PreRunRatings).Index(10);
            Map(m => m.ISRCancelledRatings).Index(11);
            Map(m => m.ISRCancelledSpots).Index(12);
            Map(m => m.RSCancelledRatings).Index(13);
            Map(m => m.RSCancelledSpots).Index(14);
            Map(m => m.OptimiserRatings).Index(15);
            Map(m => m.OptimiserBookedSpots).Index(16);
            Map(m => m.ZeroRatedSpots).Index(23);
            Map(m => m.NominalValue).Index(24);
            Map(m => m.PassThatDelivered100Percent).Index(25);
        }
    }
}
