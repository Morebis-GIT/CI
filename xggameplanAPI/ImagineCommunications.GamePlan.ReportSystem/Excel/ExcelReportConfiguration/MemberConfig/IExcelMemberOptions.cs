using System.Drawing;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles.Enums;
using ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.MemberConfig;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.MemberConfig
{
    public interface IExcelMemberOptions : IMemberOptions
    {
        int ColSpan { get; set; }
        int RowSpan { get; set; }
        bool Merge { get; set; }
        string StyleName { get; set; }
        ExcelStyle Style { get; set; }
        bool IsStyleSet { get; set; }
        string HeaderStyleName { get; set; }
        bool IsHeaderStyleSet { get; set; }
        string FormatNumber { get; set; }
        ExcelStyle HeaderStyle { get; set; }
        Color? BackgroundColor { get; set; }
        ExcelHorizontalAlignment? HAlign { get; set; }
        ExcelVerticalAlignment? VAlign { get; set; }
        bool HasAlternateBackgroundColor { get; set; }
        Color[] AlternateBackgroundColors { get; set; }
    }
}
