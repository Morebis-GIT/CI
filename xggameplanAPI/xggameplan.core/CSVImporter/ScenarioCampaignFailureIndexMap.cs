namespace xggameplan.CSVImporter
{
    public class ScenarioCampaignFailureIndexMap : OutputFileMap<ScenarioCampaignFailureImport>
    {
        public ScenarioCampaignFailureIndexMap()
        {
            Map(m => m.CampaignExternalId).Index(2);
            Map(m => m.SalesAreaNumber).Index(3);
            Map(m => m.SpotLength).Index(4);
            Map(m => m.MultipartNumber).Index(5);
            Map(m => m.StrikeWeightStartDate).Index(6);
            Map(m => m.StrikeWeightEndDate).Index(7);
            Map(m => m.DayPartStartTime).Index(8);
            Map(m => m.DayPartEndTime).Index(9);
            Map(m => m.DayPartDays).Index(10);
            Map(m => m.FailureType).Index(11);
            Map(m => m.NumberOfFailures).Index(12);
            Map(m => m.PassesEncounteringFailure).Index(14);
        }
    }
}
