namespace xggameplan.CSVImporter
{
    public class FailureHeaderMap : OutputFileMap<FailureImport>
    {
        public FailureHeaderMap()
        {
            Map(m => m.PeriodSubmissionNumber).Name("aper_no");
            Map(m => m.CampaignNumber).Name("camp_no");
            Map(m => m.SalesAreaNumberOfBooking).Name("sare_no");
            Map(m => m.SpotLength).Name("sslg_length");
            Map(m => m.MultipartNumber).Name("multipart_no");
            Map(m => m.SalesAreaNumberOfBreak).Name("brek_sare_no");
            Map(m => m.BreakDate).Name("brek_sched_date");
            Map(m => m.BreakTime).Name("brek_nom_time");
            Map(m => m.FailureType).Name("fail_type");
            Map(m => m.NumberOfFailures).Name("nbr_fails");
        }
    }
}
