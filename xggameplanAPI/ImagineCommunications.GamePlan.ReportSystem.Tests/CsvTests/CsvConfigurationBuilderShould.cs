using System.Linq;
using ImagineCommunications.GamePlan.ReportSystem.Csv.CsvReportConfiguration.EntityConfig;
using ImagineCommunications.GamePlan.ReportSystem.Tests.SampleData;
using Xunit;

namespace ImagineCommunications.GamePlan.ReportSystem.Tests.CsvTests
{
    public class CsvConfigurationBuilderShould
    {
        [Fact]
        public void CreateDefaultConfig()
        {
            var config = GetDefaultConfig();

            Assert.NotNull(config);
        }

        [Fact]
        public void CreateDefaultConfig_AllMembersConfigured()
        {
            var config = GetDefaultConfig();

            Assert.Equal( 6, config.MemberConfigurations.GetActiveOrderlyOptions().Count());
        }

        [Fact]
        public void CreateDefaultConfig_ShowHeader()
        {
            var configBuilder = new CsvConfigurationBuilder<SaleData>();
            var config = configBuilder.BuildConfiguration();

            Assert.False(config.Options.IsHideHeader);
        }

        [Fact]
        public void SetHideHeader()
        {
            var configBuilder = new CsvConfigurationBuilder<SaleData>()
                .HideHeader();
            var config = configBuilder.BuildConfiguration();

            Assert.True(config.Options.IsHideHeader);
        }

        [Fact]
        public void SetIgnoreNotMapped()
        {
            var configBuilder = new CsvConfigurationBuilder<SaleData>()
                .IgnoreNotDescribed();
            var config = configBuilder.BuildConfiguration();

            Assert.Empty(config.MemberConfigurations.GetActiveOrderlyOptions());
        }

        [Fact]
        public void DefaultUseHeaderHumanize()
        {
            var config = GetDefaultConfig();

            Assert.True(config.Options.UseHeaderHumanizer);
        }

        private static CsvReportConfiguration GetDefaultConfig()
        {
            var configBuilder = new CsvConfigurationBuilder<SaleData>();
            return configBuilder.BuildConfiguration();
        }
    }
}
