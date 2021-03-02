namespace xggameplan.CSVImporter
{
    public class BreakEfficiencyHeaderMap : OutputFileMap<BreakEfficiencyImport>
    {
        public BreakEfficiencyHeaderMap()
        {
            Map(m => m.sare_no).Name("sare_no");
            Map(m => m.brek_sched_date).Name("brek_sched_date");
            Map(m => m.brek_nom_time).Name("brek_nom_time");
            Map(m => m.break_no).Name("break_no");
            Map(m => m.demo_no).Name("demo_no");
            Map(m => m.eff).Name("eff");
        }
    }
}
