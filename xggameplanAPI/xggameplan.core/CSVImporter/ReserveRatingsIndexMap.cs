namespace xggameplan.CSVImporter
{
    public class ReserveRatingsIndexMap : OutputFileMap<ReserveRatingsImport>
    {
        public ReserveRatingsIndexMap()
        {
            Map(m => m.sare_no).Index(0);
            Map(m => m.demo_no).Index(1);
            Map(m => m.rtgs_tot).Index(2);
            Map(m => m.rtgs_pre).Index(3);
            Map(m => m.rtgs_post).Index(4);
            Map(m => m.rtgs_resv).Index(5);
        }
    }
}
