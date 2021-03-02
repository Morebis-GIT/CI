using System.Collections.Generic;

namespace xggameplan.CSVImporter
{
    /// <summary>
    /// Class for SPOT.OUT
    /// </summary>
    public class SpotImport : SpotImportBase
    {
        public int break_no { get; set; }
        public int? prdp_no { get; set; }
        public int? rcdp_no { get; set; }
        public long? schd_no { get; set; }
        public long? option_expiry_date { get; set; }    
        public string root_clsh_code { get; set; }
        public string clsh_code { get; set; }
        public int? pree_tor_status { get; set; }
        public int? pree_status { get; set; }
        public string mbru_code { get; set; }
        public string sptt_code { get; set; }
        public int? rcrd_ratecard_no { get; set; }
        public int bsar_no { get; set; }
        public int rcrd_sare_no { get; set; }
        public string added_value_yn { get; set; }
        public string posn_surch_yn { get; set; }
        public double starting_price { get; set; }
        public double psd_price { get; set; }
        public double base_leng_stt_price { get; set; }
        public double base_leng_psd_price { get; set; }
        public double? break_price { get; set; }
        public string entered_price_yn { get; set; }
        public string price_locked_yn { get; set; }
        public string client_picked { get; set; }
        public string prog_lock_yn { get; set; }
        public int pass_no { get; set; }
        public long deal_no { get; set; }

        public static Dictionary<long, SpotImport> IndexListBySpotNo(IEnumerable<SpotImport> spotImports)
        {
            Dictionary<long, SpotImport> spotImportsIndexed = new Dictionary<long, SpotImport>();
            foreach (var spotImport in spotImports)
            {
                if (!spotImportsIndexed.ContainsKey(spotImport.spot_no))
                {
                    spotImportsIndexed.Add(spotImport.spot_no, spotImport);
                }
            }
            return spotImportsIndexed;
        }
    }
}
