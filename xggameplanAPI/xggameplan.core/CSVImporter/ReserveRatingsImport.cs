namespace xggameplan.CSVImporter
{
    /// <summary>
    /// RESV_RTGS file import model
    /// </summary>
    public class ReserveRatingsImport
    {
        public int sare_no { get; set; }
        public int demo_no { get; set; }
        public double rtgs_tot { get; set; }
        public double rtgs_pre { get; set; }
        public double rtgs_post { get; set; }
        public double rtgs_resv { get; set; }
    }
}
