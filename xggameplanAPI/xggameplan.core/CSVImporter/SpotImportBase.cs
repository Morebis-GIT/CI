namespace xggameplan.CSVImporter
{
    public abstract class SpotImportBase
    {
        public long spot_no { get; set; }
        public long camp_no { get; set; }
        public int sare_no { get; set; }
        public int sslg_length { get; set; }
        public string status { get; set; }
        public long prod_code { get; set; }
        public int demo_no { get; set; }
        public string mpart_spot_ind { get; set; }
        public int? bkpo_posn_reqm { get; set; }
        public long brek_sched_date { get; set; }
        public long brek_nom_time { get; set; }
        public int spot_sare_no { get; set; }
        public double price_factor { get; set; }
        public double efficiency { get; set; }
        public int eff_level { get; set; }
        public long? optimisation_index { get; set; }
        public long? optimisation_limit { get; set; }
    }
}
