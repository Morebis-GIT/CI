namespace xggameplan.CSVImporter
{
    public class ScenarioCampaignFailureImport
    {
        public string CampaignExternalId { get; set; }
        public int SalesAreaNumber { get; set; }
        public int SpotLength { get; set; }
        public int MultipartNumber { get; set; }
        public long StrikeWeightStartDate { get; set; }
        public long StrikeWeightEndDate { get; set; }
        public long DayPartStartTime { get; set; }
        public long DayPartEndTime { get; set; }
        public string DayPartDays { get; set; }
        public int FailureType { get; set; }
        public long NumberOfFailures { get; set; }
        public long PassesEncounteringFailure { get; set; }
    }
}
