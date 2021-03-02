using System.Drawing;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.MemberConfig;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles.Enums;
using Xunit;

namespace ImagineCommunications.GamePlan.ReportSystem.Tests.ExcelTests
{
    public class ExcelMemberOptionsBuilderShould
    {
        [Fact]
        public void SetIgnore()
        {
            var builder = CreateExcelMemberOptionsBuilder();
            _ = builder.Ignore();
            var option = builder.Build();

            Assert.True(option.Ignore);
        }

        [Fact]
        public void SetHeader()
        {
            var headerName = "header";
            var builder = CreateExcelMemberOptionsBuilder();
            _ = builder.Header(headerName);
            var option = builder.Build();

            Assert.Equal(headerName, option.Header);
            Assert.True(option.IsHeader);
        }

        [Fact]
        public void SetFormatter()
        {
            var builder = CreateExcelMemberOptionsBuilder();
            _ = builder.Formatter(o => o.ToString());
            var option = builder.Build();

            Assert.NotNull(option.FormatterExpression);
        }

        [Fact]
        public void SetMerge()
        {
            var builder = CreateExcelMemberOptionsBuilder();
            _ = builder.Merge();
            var option = builder.Build();

            Assert.True(option.Merge);
        }

        [Fact]
        public void SetBackground()
        {
            var builder = CreateExcelMemberOptionsBuilder();
            _ = builder.Background(Color.White);
            var option = builder.Build();

            Assert.NotNull(option.BackgroundColor);
            Assert.Equal(Color.White, option.BackgroundColor.Value);
        }

        [Fact]
        public void SetColSpan()
        {
            var colSpan = 2;
            var builder = CreateExcelMemberOptionsBuilder();
            _ = builder.ColSpan(colSpan);
            var option = builder.Build();

            Assert.Equal(colSpan, option.ColSpan);
        }

        [Fact]
        public void DefaultColSpan()
        {
            var builder = CreateExcelMemberOptionsBuilder();
            var option = builder.Build();

            Assert.Equal(1, option.ColSpan);
        }

        [Fact]
        public void SetHAlign()
        {
            var builder = CreateExcelMemberOptionsBuilder();
            _ = builder.HAlign(ExcelHorizontalAlignment.Center);
            var option = builder.Build();

            Assert.Equal(ExcelHorizontalAlignment.Center, option.HAlign);
        }

        [Fact]
        public void SetVAlign()
        {
            var builder = CreateExcelMemberOptionsBuilder();
            _ = builder.VAlign(ExcelVerticalAlignment.Center);
            var option = builder.Build();

            Assert.Equal(ExcelVerticalAlignment.Center, option.VAlign);
        }

        private static ExcelMemberOptionsBuilder CreateExcelMemberOptionsBuilder()
        {
            return new ExcelMemberOptionsBuilder("memberName");
        }
    }
}
