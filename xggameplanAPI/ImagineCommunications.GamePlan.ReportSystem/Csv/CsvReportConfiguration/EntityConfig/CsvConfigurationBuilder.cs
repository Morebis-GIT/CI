using ImagineCommunications.GamePlan.ReportSystem.Csv.CsvReportConfiguration.MemberConfig;
using ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.EntityConfig;
using ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.MemberConfig;

namespace ImagineCommunications.GamePlan.ReportSystem.Csv.CsvReportConfiguration.EntityConfig
{
    public class CsvConfigurationBuilder<TEntity>
        : ConfigurationBuilder<TEntity, CsvMemberOptionsBuilder, CsvConfigurationBuilder<TEntity>, CsvReportConfiguration, MemberOptions>
        where TEntity : class
    {
        public CsvConfigurationBuilder() : base(new ConfigurationOptions())
        {
        }

        public CsvConfigurationBuilder(IConfigurationOptions options) : base(options)
        {
        }

        public override CsvReportConfiguration BuildConfiguration()
        {
            return BuildConfiguration<IConfigurationOptions>();
        }
    }
}
