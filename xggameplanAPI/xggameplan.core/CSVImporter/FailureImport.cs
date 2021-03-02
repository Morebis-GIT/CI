namespace xggameplan.CSVImporter
{
    public class FailureImport
    {
        public long PeriodSubmissionNumber { get; set; }
        public long CampaignNumber { get; set; }
        public int SalesAreaNumberOfBooking { get; set; }
        public int SpotLength { get; set; }
        public int MultipartNumber { get; set; }
        public int SalesAreaNumberOfBreak { get; set; }
        public string BreakDate { get; set; }
        public string BreakTime { get; set; }        
        public int FailureType { get; set; }
        public long NumberOfFailures { get; set; }
    }
}