namespace xggameplan.CSVImporter
{
    public class SpotReqmHeaderMap : OutputFileMap<SpotReqmImport>
    {
        public SpotReqmHeaderMap()
        {
            Map(m => m.aper_no).Name("aper_no");
            Map(m => m.spot_no).Name("spot_no");
            Map(m => m.camp_no).Name("camp_no");
            Map(m => m.sare_no).Name("sare_no");
            Map(m => m.sslg_length).Name("sslg_length");
            Map(m => m.status).Name("status");
            Map(m => m.prod_code).Name("prod_code");
            Map(m => m.demo_no).Name("demo_no");
            Map(m => m.mpart_spot_ind).Name("mpart_spot_ind");
            Map(m => m.bkpo_posn_reqm).ConvertUsing(row => ConvertValue<int?>(row.GetField("bkpo_posn_reqm")));
            Map(m => m.brek_sare_no).Name("brek_sare_no");
            Map(m => m.brek_sched_date).Name("brek_sched_date");
            Map(m => m.brek_nom_time).Name("brek_nom_time");
            Map(m => m.spot_sare_no).Name("spot_sare_no");
            Map(m => m.day_no).Name("day_no");
            Map(m => m.prog_no).Name("prog_no");
            Map(m => m.epis_no).Name("epis_no");
            Map(m => m.prgc_no).Name("prgc_no");
            Map(m => m.posn_in_prog).Name("posn_in_prog");
            Map(m => m.btyp_code).Name("btyp_code");
            Map(m => m.price_factor).Name("price_factor");
            Map(m => m.nominal_price).Name("nominal_price");
            Map(m => m.rating).Name("rating");
            Map(m => m.base_rating).Name("base_rating");
            Map(m => m.efficiency).Name("efficiency");
            Map(m => m.eff_level).Name("eff_level");
            Map(m => m.optimisation_index).ConvertUsing(row => ConvertValue<int?>(row.GetField("optimisation_index")));
            Map(m => m.optimisation_limit).ConvertUsing(row => ConvertValue<int?>(row.GetField("optimisation_limit")));
            Map(m => m.abdn_no).Name("abdn_no");
            Map(m => m.brek_external_ref).Name("brek_external_ref");
            Map(m => m.pass_sequence_no).Name("pass_sequence");
            Map(m => m.campaign_pass_priority).Name("camp_pass_priority");
            Map(m => m.rank_of_efficiency).Name("rank_of_effe");
            Map(m => m.rank_of_campaign).Name("rank_of_camp");
            Map(m => m.campaign_weighting).Name("camp_weighting");
            Map(m => m.tarps).Name("tarps");
            Map(m => m.client_picked).Name("client_picked");
        }
    }
}
