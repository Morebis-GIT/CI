using ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration;
using ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.EntityConfig;
using ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.MemberConfig;

namespace ImagineCommunications.GamePlan.ReportSystem.Csv.CsvReportConfiguration.EntityConfig
{
    public interface ICsvReportConfiguration : IReportConfigurations<IConfigurationOptions, MemberOptions>
    {
    }
}
