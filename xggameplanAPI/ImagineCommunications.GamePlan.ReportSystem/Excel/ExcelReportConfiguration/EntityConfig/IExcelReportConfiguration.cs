using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.MemberConfig;
using ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.EntityConfig
{
    public interface IExcelReportConfiguration
        : IReportConfigurations<IExcelConfigurationOptions, ExcelMemberOptions>
    {
    }
}
