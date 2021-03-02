namespace xggameplan.CSVImporter
{
    /// <summary>
    /// Base Ratings Index Mapping
    /// </summary>
    public class BaseRatingsIndexMap : OutputFileMap<BaseRatingsImport>
    {
        public BaseRatingsIndexMap()
        {
            Map(m => m.sare_no).Index(0);
            Map(m => m.base_rtgs_tot).Index(1);
            Map(m => m.base_rtgs_pre).Index(2);
            Map(m => m.base_rtgs_post).Index(3);
            Map(m => m.total_duration).Index(4);
            Map(m => m.total_open_avail_pre).Index(5);
            Map(m => m.total_open_avail_post).Index(6);
            Map(m => m.no_of_breaks).Index(7);
        }
    }
}
