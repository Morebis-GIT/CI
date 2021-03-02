using System.Drawing;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles.Enums;
using ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.MemberConfig;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.MemberConfig
{
    public interface IBaseExcelMemberOptionsBuilder<out TBuilder, out TMemberOption>
        : IMemberOptionsBuilder<TBuilder, TMemberOption>
            where TMemberOption : class, IMemberOptions
            where TBuilder : IMemberOptionsBuilder<TBuilder, TMemberOption>
    {
        TBuilder ColSpan(int value);
        TBuilder Merge();
        TBuilder Merge(bool merge);
        TBuilder Style(string predefinedStyleName);
        TBuilder Style(ExcelStyle style);
        TBuilder HeaderStyle(string styleName);
        TBuilder HeaderStyle(ExcelStyle style);
        TBuilder Background(Color color);
        TBuilder HAlign(ExcelHorizontalAlignment alignment);
        TBuilder VAlign(ExcelVerticalAlignment alignment);
    }
}
