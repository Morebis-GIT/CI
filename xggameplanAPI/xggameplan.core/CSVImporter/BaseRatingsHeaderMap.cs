namespace xggameplan.CSVImporter
{
    /// <summary>
    /// Base Ratings Mapping
    /// </summary>
    public class BaseRatingsHeaderMap : OutputFileMap<BaseRatingsImport>
    {
        public BaseRatingsHeaderMap()
        {
            Map(m => m.sare_no).Name("sare_no");
            Map(m => m.base_rtgs_tot).Name("base_rtgs_tot");
            Map(m => m.base_rtgs_pre).Name("base_rtgs_pre");
            Map(m => m.base_rtgs_post).Name("base_rtgs_post");
            Map(m => m.total_duration).Name("total_duration");
            Map(m => m.total_open_avail_pre).Name("total_open_avail_pre");
            Map(m => m.total_open_avail_post).Name("total_open_avail_post");
            Map(m => m.no_of_breaks).Name("nbr_breks");
        }
    }
}
