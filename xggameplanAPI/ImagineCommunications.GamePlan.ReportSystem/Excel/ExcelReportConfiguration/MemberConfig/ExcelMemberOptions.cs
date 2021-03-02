using System.Drawing;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles.Enums;
using ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.MemberConfig;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.MemberConfig
{
    public class ExcelMemberOptions: MemberOptions, IExcelMemberOptions
    {
        public int ColSpan { get; set; } = 1;
        public int RowSpan { get; set; } = 1;
        public bool Merge { get; set; }
        public string StyleName { get; set; }
        public ExcelStyle Style { get; set; }
        public bool IsStyleSet { get; set; }
        public string HeaderStyleName { get; set; }
        public ExcelStyle HeaderStyle { get; set; }
        public bool IsHeaderStyleSet { get; set; }
        public Color? BackgroundColor { get; set; }
        public ExcelHorizontalAlignment? HAlign { get; set; }
        public ExcelVerticalAlignment? VAlign { get; set; }
        public string FormatNumber { get; set; }
        public bool HasAlternateBackgroundColor { get; set; }
        public Color[] AlternateBackgroundColors { get; set; }

        public ExcelMemberOptions()
        {
        }

        public ExcelMemberOptions(string memberName) : base(memberName)
        {
        }
    }
}
