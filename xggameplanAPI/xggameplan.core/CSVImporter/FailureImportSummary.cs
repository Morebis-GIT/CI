namespace xggameplan.CSVImporter
{
    public class FailureImportSummary
    {    
        public long Campaign { get; set; }             
        public int Type { get; set; }
        public long Failures { get; set; }
        public int SalesAreaNumberOfBooking { get; set; }

        public int SpotLength { get; set; }
        public int MultipartNumber { get; set; }
        public string BreakDate { get; set; }
        public string BreakTime { get; set; }
    }
}