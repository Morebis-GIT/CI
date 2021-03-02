namespace xggameplan.CSVImporter
{
    public class SpotReqmIndexMap : OutputFileMap<SpotReqmImport>
    {
        public SpotReqmIndexMap()
        {
            Map(m => m.aper_no).Index(0);
            Map(m => m.spot_no).Index(1);
            Map(m => m.camp_no).Index(2);
            Map(m => m.sare_no).Index(3);
            Map(m => m.sslg_length).Index(4);
            Map(m => m.status).Index(5);
            Map(m => m.prod_code).Index(6);
            Map(m => m.demo_no).Index(7);
            Map(m => m.mpart_spot_ind).Index(8);
            Map(m => m.bkpo_posn_reqm).ConvertUsing(row => ConvertValue<int?>(row.GetField(9)));
            Map(m => m.brek_sare_no).Index(10);
            Map(m => m.brek_sched_date).Index(11);
            Map(m => m.brek_nom_time).Index(12);
            Map(m => m.spot_sare_no).Index(13);
            Map(m => m.day_no).Index(14);
            Map(m => m.prog_no).Index(15);
            Map(m => m.epis_no).Index(16);
            Map(m => m.prgc_no).Index(17);
            Map(m => m.posn_in_prog).Index(18);
            Map(m => m.btyp_code).Index(19);
            Map(m => m.price_factor).Index(20);
            Map(m => m.nominal_price).Index(21);
            Map(m => m.rating).Index(22);
            Map(m => m.base_rating).Index(23);
            Map(m => m.efficiency).Index(24);
            Map(m => m.eff_level).Index(25);
            Map(m => m.optimisation_index).ConvertUsing(row => ConvertValue<int?>(row.GetField(26)));
            Map(m => m.optimisation_limit).ConvertUsing(row => ConvertValue<int?>(row.GetField(27)));
            Map(m => m.abdn_no).Index(28);
            Map(m => m.brek_external_ref).Index(32);
            Map(m => m.pass_sequence_no).Index(33);
            Map(m => m.campaign_pass_priority).Index(35);
            Map(m => m.rank_of_efficiency).Index(36);
            Map(m => m.rank_of_campaign).Index(37);
            Map(m => m.campaign_weighting).Index(38);
            Map(m => m.tarps).Index(44);
            Map(m => m.client_picked).Index(45);
        }
    }
}
