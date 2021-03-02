using System.Collections.Generic;
using System.Drawing;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles;
using ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.EntityConfig;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.EntityConfig
{
    public class ExcelConfigurationOptions: ConfigurationOptions, IExcelConfigurationOptions
    {
        public ExcelStyle HeaderStyle { get; set; }
        public string HeaderStyleName { get; set; }
        public bool IsSetHeaderStyle { get; set; }
        public ExcelStyle DefaultStyle { get; set; }
        public string DefaultStyleName { get; set; }
        public bool IsSetDefaultStyle { get; set; }
        public bool HasAlternateBackgroundColor { get; set; }
        public IReadOnlyList<KeyValuePair<string, Color>> AlternateBackgroundColors { get; set; }
    }
}
