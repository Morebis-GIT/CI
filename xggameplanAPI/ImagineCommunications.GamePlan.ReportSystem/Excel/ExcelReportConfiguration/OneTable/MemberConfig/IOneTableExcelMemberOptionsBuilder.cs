using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.MemberConfig;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.OneTable.MemberConfig
{
    public interface IOneTableExcelMemberOptionsBuilder
        : IBaseExcelMemberOptionsBuilder<OneTableExcelMemberOptionsBuilder, OneTableExcelMemberOptions>
    {
        IOneTableExcelMemberOptionsBuilder Width(double width);
        IOneTableExcelMemberOptionsBuilder AutoFitColumn(bool isAutoFitColumn);
        IOneTableExcelMemberOptionsBuilder Format(string format);
    }
}
