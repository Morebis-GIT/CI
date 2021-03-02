using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles.Enums;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles
{
    public class ExcelStyle
    {
        public ExcelFont Font { get; set; }
        public ExcelBorder Border { get; set; }
        public ExcelFill Fill { get; set; }
        public ExcelHorizontalAlignment? HorizontalAlignment { get; set; }
        public ExcelVerticalAlignment? VerticalAlignment { get; set; }
        public bool WrapText { get; set; }
        public string NumberFormat { get; set; }
    }
}
