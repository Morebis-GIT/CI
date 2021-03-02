using System.Drawing;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles.Enums;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles
{
    public class ExcelFont
    {
        public Font Font { get; set; }
        public Color? FontColor { get; set; }
        public ExcelVerticalAlignmentFont? VerticalAlignmentFont { get; set; }
    }
}