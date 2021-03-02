using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.EntityConfig;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.OneTable.MemberConfig;
using ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.OneTable.EntityConfig
{
    public interface IOneTableExcelReportConfiguration
        : IReportConfigurations<IExcelConfigurationOptions, OneTableExcelMemberOptions>
    {
    }
}
