namespace xggameplan.CSVImporter
{
    public class BreakEfficiencyIndexMap : OutputFileMap<BreakEfficiencyImport>
    {
        public BreakEfficiencyIndexMap()
        {
            Map(m => m.sare_no).Index(0);
            Map(m => m.brek_sched_date).Index(1);
            Map(m => m.brek_nom_time).Index(2);
            Map(m => m.break_no).Index(3);
            Map(m => m.demo_no).Index(4);
            Map(m => m.eff).Index(5);
        }
    }
}
