namespace xggameplan.CSVImporter
{
    /// <summary>
    /// Class for BASE_RTGS.OUT
    /// </summary>
    public class BaseRatingsImport
    {
        public int sare_no { get; set; }
        public double base_rtgs_tot { get; set; }
        public double base_rtgs_pre { get; set; }
        public double base_rtgs_post { get; set; }
        public long total_duration { get; set; }
        public long total_open_avail_pre { get; set; }
        public long total_open_avail_post { get; set; }        
        public int no_of_breaks { get; set; }
    }
}