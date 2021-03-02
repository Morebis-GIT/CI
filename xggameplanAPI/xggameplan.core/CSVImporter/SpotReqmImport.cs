namespace xggameplan.CSVImporter
{
    /// <summary>
    /// Class for LMKII_SPOT_REQM.OUT
    /// </summary>
    public class SpotReqmImport : SpotImportBase
    {
        public int abdn_no { get; set; }
        public int pass_sequence_no { get; set; }
        public int campaign_pass_priority { get; set; }
        public long rank_of_efficiency { get; set; }
        public int rank_of_campaign { get; set; }
        public double campaign_weighting { get; set; }
        public long prog_no { get; set; }
        public int day_no { get; set; }
        public long aper_no { get; set; }
        public int brek_sare_no { get; set; }
        public int epis_no { get; set; }
        public int prgc_no { get; set; }
        public string posn_in_prog { get; set; }
        public string btyp_code { get; set; }
        public double nominal_price { get; set; }
        public double rating { get; set; }
        public double base_rating { get; set; }
        public double tarps { get; set; }
        public string brek_external_ref { get; set; }
        public string client_picked { get; set; }
    }
}
