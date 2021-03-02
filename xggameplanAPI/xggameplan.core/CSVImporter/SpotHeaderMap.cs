namespace xggameplan.CSVImporter
{
    public class SpotHeaderMap : OutputFileMap<SpotImport>
    {
        public SpotHeaderMap()
        {
            Map(m => m.sare_no).Name("sare_no");
            Map(m => m.brek_sched_date).Name("brek_sched_date");
            Map(m => m.brek_nom_time).Name("brek_nom_time");
            Map(m => m.spot_no).Name("spot_no");
            Map(m => m.break_no).Name("break_no");
            Map(m => m.demo_no).Name("demo_no");
            Map(m => m.prdp_no).ConvertUsing(row => ConvertValue<int?>(row.GetField("prdp_no")));     
            Map(m => m.rcdp_no).ConvertUsing(row => ConvertValue<int?>(row.GetField("rcdp_no")));     
            Map(m => m.schd_no).ConvertUsing(row => ConvertValue<long?>(row.GetField("schd_no")));      
            Map(m => m.option_expiry_date).ConvertUsing(row => ConvertValue<long?>(row.GetField("option_expiry_date")));    
            Map(m => m.status).Name("status");
            Map(m => m.prod_code).Name("prod_code");
            Map(m => m.root_clsh_code).Name("root_clsh_code");
            Map(m => m.clsh_code).Name("clsh_code");
            Map(m => m.mpart_spot_ind).Name("mpart_spot_ind");
            Map(m => m.pree_tor_status).ConvertUsing(row => ConvertValue<int?>(row.GetField("pree_tor_status")));    
            Map(m => m.pree_status).ConvertUsing(row => ConvertValue<int?>(row.GetField("pree_status")));   
            Map(m => m.mbru_code).Name("mbru_code");
            Map(m => m.sptt_code).Name("sptt_code");
            Map(m => m.bkpo_posn_reqm).ConvertUsing(row => ConvertValue<int?>(row.GetField("bkpo_posn_reqm")));
            Map(m => m.deal_no).Name("deal_no");
            Map(m => m.camp_no).Name("camp_no");
            Map(m => m.sslg_length).Name("sslg_length");
            Map(m => m.rcrd_ratecard_no).ConvertUsing(row => ConvertValue<int?>(row.GetField("rcrd_ratecard_no"))); 
            Map(m => m.bsar_no).Name("bsar_no");
            Map(m => m.spot_sare_no).Name("spot_sare_no");
            Map(m => m.price_factor).Name("price_factor");
            Map(m => m.rcrd_sare_no).Name("rcrd_sare_no");
            Map(m => m.added_value_yn).Name("added_value_yn");
            Map(m => m.posn_surch_yn).Name("posn_surch_yn");
            Map(m => m.starting_price).Name("starting_price");
            Map(m => m.psd_price).Name("psd_price");
            Map(m => m.base_leng_stt_price).Name("base_leng_stt_price");
            Map(m => m.base_leng_psd_price).Name("base_leng_psd_price");
            Map(m => m.break_price).ConvertUsing(row => ConvertValue<double?>(row.GetField("break_price")));
            Map(m => m.entered_price_yn).Name("entered_price_yn");
            Map(m => m.price_locked_yn).Name("price_locked_yn");
            Map(m => m.client_picked).Name("client_picked");
            Map(m => m.prog_lock_yn).Name("prog_lock_yn");
            Map(m => m.efficiency).Name("efficiency");
            Map(m => m.optimisation_index).ConvertUsing(row => ConvertValue<long?>(row.GetField("optimisation_index")));
            Map(m => m.optimisation_limit).ConvertUsing(row => ConvertValue<long?>(row.GetField("optimisation_limit")));
            Map(m => m.eff_level).Name("eff_level");
            Map(m => m.pass_no).Name("pass_no");
        }
    }
}
