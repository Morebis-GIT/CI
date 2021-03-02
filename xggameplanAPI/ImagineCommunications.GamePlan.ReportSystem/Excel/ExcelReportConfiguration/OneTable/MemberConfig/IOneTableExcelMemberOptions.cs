using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.MemberConfig;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.OneTable.MemberConfig
{
    public interface IOneTableExcelMemberOptions:IExcelMemberOptions
    {
        double Width { get; set; }

        bool IsAutoFitColumn { get; set; }

        string Format { get; set; }
    }
}
