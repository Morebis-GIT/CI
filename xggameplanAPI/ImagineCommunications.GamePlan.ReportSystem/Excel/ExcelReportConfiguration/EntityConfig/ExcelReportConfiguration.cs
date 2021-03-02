using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.MemberConfig;
using ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration;
using ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.MemberConfig;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.EntityConfig
{
    public class ExcelReportConfiguration
        : ReportConfigurations<IExcelConfigurationOptions, ExcelMemberOptions>
            , IExcelReportConfiguration
    {
        public ExcelReportConfiguration(IExcelConfigurationOptions options, MemberConfigurationDictionary<ExcelMemberOptions> memberConfigurations)
            : base(options, memberConfigurations)
        {
        }
    }
}
