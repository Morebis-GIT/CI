namespace xggameplan.CSVImporter
{
    public class FailureIndexMap : OutputFileMap<FailureImport>
    {
        public FailureIndexMap()
        {
            Map(m => m.PeriodSubmissionNumber).Index(0);
            Map(m => m.CampaignNumber).Index(1);
            Map(m => m.SalesAreaNumberOfBooking).Index(2);
            Map(m => m.SpotLength).Index(3);
            Map(m => m.MultipartNumber).Index(4);
            Map(m => m.SalesAreaNumberOfBreak).Index(5);
            Map(m => m.BreakDate).Index(6);
            Map(m => m.BreakTime).Index(7);
            Map(m => m.FailureType).Index(8);
            Map(m => m.NumberOfFailures).Index(9);
        }
    }
}
