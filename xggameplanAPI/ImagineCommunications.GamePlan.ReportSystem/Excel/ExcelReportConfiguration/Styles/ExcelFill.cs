using System.Drawing;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles.Enums;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles
{
    public class ExcelFill
    {
        public ExcelFillStyle? PatternType { get; set; }
        public Color? PatternColor { get; set; }
        public Color? BackgroundColor { get; set; }
    }
}
