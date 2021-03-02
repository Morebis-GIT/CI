using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.EntityConfig;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.OneTable.MemberConfig;
using ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration;
using ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.MemberConfig;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.OneTable.EntityConfig
{
    public class OneTableExcelReportConfiguration
        : ReportConfigurations<IExcelConfigurationOptions, OneTableExcelMemberOptions>
            , IOneTableExcelReportConfiguration
    {
        public OneTableExcelReportConfiguration(IExcelConfigurationOptions options, MemberConfigurationDictionary<OneTableExcelMemberOptions> memberConfigurations)
            : base(options, memberConfigurations)
        {
        }
    }
}
