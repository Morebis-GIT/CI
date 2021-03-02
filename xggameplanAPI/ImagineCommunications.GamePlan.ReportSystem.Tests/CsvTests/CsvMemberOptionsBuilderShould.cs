using ImagineCommunications.GamePlan.ReportSystem.Csv.CsvReportConfiguration.MemberConfig;
using Xunit;

namespace ImagineCommunications.GamePlan.ReportSystem.Tests.CsvTests
{
    public class CsvMemberOptionsBuilderShould
    {
        [Fact]
        public void SetIgnore()
        {
            var builder = CreateCsvMemberOptionsBuilder();
            _ = builder.Ignore();
            var option = builder.Build();

            Assert.True(option.Ignore);
        }

        [Fact]
        public void SetHeader()
        {
            var headerName = "header";

            var builder = CreateCsvMemberOptionsBuilder();
            _ = builder.Header(headerName);
            var option = builder.Build();

            Assert.Equal(headerName, option.Header);
            Assert.True(option.IsHeader);
        }

        [Fact]
        public void SetFormatter()
        {
            var builder = CreateCsvMemberOptionsBuilder();
            _ = builder.Formatter(o => o.ToString());
            var option = builder.Build();

            Assert.NotNull(option.FormatterExpression);
        }

        private static CsvMemberOptionsBuilder CreateCsvMemberOptionsBuilder()
        {
            var builder = new CsvMemberOptionsBuilder("memberName");
            return builder;
        }
    }
}
