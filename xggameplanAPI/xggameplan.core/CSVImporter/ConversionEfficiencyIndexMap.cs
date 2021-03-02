namespace xggameplan.CSVImporter
{
    public class ConversionEfficiencyIndexMap : OutputFileMap<ConversionEfficiencyImport>
    {
        public ConversionEfficiencyIndexMap()
        {
            Map(m => m.demo_no).Index(0);
            Map(m => m.rtgs_30).Index(1);
            Map(m => m.base_rtgs_30).Index(2);
            Map(m => m.totr_rtgs_30).Index(3);
            Map(m => m.totr_base_rtgs_30).Index(4);
            Map(m => m.natural_conversion).Index(5);
            Map(m => m.actual_conversion).Index(6);
            Map(m => m.conv_eff_index).Index(7);
        }
    }
}
