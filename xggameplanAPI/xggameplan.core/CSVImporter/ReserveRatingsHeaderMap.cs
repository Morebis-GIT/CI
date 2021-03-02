namespace xggameplan.CSVImporter
{
    public class ReserveRatingsHeaderMap : OutputFileMap<ReserveRatingsImport>
    {
        public ReserveRatingsHeaderMap()
        {
            Map(m => m.sare_no).Name("sare_no");
            Map(m => m.demo_no).Name("demo_no");
            Map(m => m.rtgs_tot).Name("rtgs_tot");
            Map(m => m.rtgs_pre).Name("rtgs_pre");
            Map(m => m.rtgs_post).Name("rtgs_post");
            Map(m => m.rtgs_resv).Name("rtgs_resv");
        }
    }
}
