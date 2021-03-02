using ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration;
using ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.EntityConfig;
using ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.MemberConfig;

namespace ImagineCommunications.GamePlan.ReportSystem.Csv.CsvReportConfiguration.EntityConfig
{
    public class CsvReportConfiguration : ReportConfigurations<IConfigurationOptions, MemberOptions>, ICsvReportConfiguration
    {
        public CsvReportConfiguration(IConfigurationOptions options, MemberConfigurationDictionary<MemberOptions> memberConfigurations) : base(options, memberConfigurations)
        {
        }
    }
}
