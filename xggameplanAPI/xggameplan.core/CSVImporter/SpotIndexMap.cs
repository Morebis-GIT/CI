namespace xggameplan.CSVImporter
{
    public class SpotIndexMap : OutputFileMap<SpotImport>
    {
        public SpotIndexMap()
        {
            Map(m => m.sare_no).Index(0);
            Map(m => m.brek_sched_date).Index(1);
            Map(m => m.brek_nom_time).Index(2);
            Map(m => m.spot_no).Index(3);
            Map(m => m.break_no).Index(4);
            Map(m => m.demo_no).Index(5);
            Map(m => m.prdp_no).ConvertUsing(row => ConvertValue<int?>(row.GetField(6)));
            Map(m => m.rcdp_no).ConvertUsing(row => ConvertValue<int?>(row.GetField(7)));
            Map(m => m.schd_no).ConvertUsing(row => ConvertValue<long?>(row.GetField(8)));
            Map(m => m.option_expiry_date).ConvertUsing(row => ConvertValue<long?>(row.GetField(9)));
            Map(m => m.status).Index(10);
            Map(m => m.prod_code).Index(11);
            Map(m => m.root_clsh_code).Index(12);
            Map(m => m.clsh_code).Index(13);
            Map(m => m.mpart_spot_ind).Index(14);
            Map(m => m.pree_tor_status).ConvertUsing(row => ConvertValue<int?>(row.GetField(15)));
            Map(m => m.pree_status).ConvertUsing(row => ConvertValue<int?>(row.GetField(16)));
            Map(m => m.mbru_code).Index(17);
            Map(m => m.sptt_code).Index(18);
            Map(m => m.bkpo_posn_reqm).ConvertUsing(row => ConvertValue<int?>(row.GetField(19)));
            Map(m => m.deal_no).Index(20);
            Map(m => m.camp_no).Index(21);
            Map(m => m.sslg_length).Index(22);
            Map(m => m.rcrd_ratecard_no).ConvertUsing(row => ConvertValue<int?>(row.GetField(23)));
            Map(m => m.bsar_no).Index(24);
            Map(m => m.spot_sare_no).Index(25);
            Map(m => m.price_factor).Index(26);
            Map(m => m.rcrd_sare_no).Index(27);
            Map(m => m.added_value_yn).Index(28);
            Map(m => m.posn_surch_yn).Index(29);
            Map(m => m.starting_price).Index(30);
            Map(m => m.psd_price).Index(31);
            Map(m => m.base_leng_stt_price).Index(32);
            Map(m => m.base_leng_psd_price).Index(33);
            Map(m => m.break_price).ConvertUsing(row => ConvertValue<double?>(row.GetField(34)));
            Map(m => m.entered_price_yn).Index(35);
            Map(m => m.price_locked_yn).Index(36);
            Map(m => m.client_picked).Index(37);
            Map(m => m.prog_lock_yn).Index(38);
            Map(m => m.efficiency).Index(39);
            Map(m => m.optimisation_index).ConvertUsing(row => ConvertValue<long?>(row.GetField(40)));
            Map(m => m.optimisation_limit).ConvertUsing(row => ConvertValue<long?>(row.GetField(41)));
            Map(m => m.eff_level).Index(42);
            Map(m => m.pass_no).Index(43);
        }
    }
}
