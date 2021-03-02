namespace xggameplan.CSVImporter
{
    public class ConversionEfficiencyHeaderMap : OutputFileMap<ConversionEfficiencyImport>
    {
        public ConversionEfficiencyHeaderMap()
        {
            Map(m => m.demo_no).Name("demo_no");
            Map(m => m.rtgs_30).Name("rtgs_30");
            Map(m => m.base_rtgs_30).Name("base_rtgs_30");
            Map(m => m.totr_rtgs_30).Name("totr_rtgs_30");
            Map(m => m.totr_base_rtgs_30).Name("totr_base_rtgs_30");
            Map(m => m.natural_conversion).Name("natural_conversion");
            Map(m => m.actual_conversion).Name("actual_conversion");
            Map(m => m.conv_eff_index).Name("conv_eff_index");
        }
    }
}
