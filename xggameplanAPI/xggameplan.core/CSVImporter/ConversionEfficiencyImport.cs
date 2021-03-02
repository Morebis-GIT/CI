namespace xggameplan.CSVImporter
{
    /// <summary>
    /// RESV_RTGS file import model
    /// </summary>
    public class ConversionEfficiencyImport
    {
        public int demo_no { get; set; }
        public double rtgs_30 { get; set; }
        public double base_rtgs_30 { get; set; }
        public double totr_rtgs_30 { get; set; }
        public double totr_base_rtgs_30 { get; set; }
        public double natural_conversion { get; set; }
        public double actual_conversion { get; set; }
        public double conv_eff_index { get; set; }
    }
}
