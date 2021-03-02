using System.Drawing;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles.Enums;
using ImagineCommunications.GamePlan.ReportSystem.Excel.SheetBuilder.BlockBuilder;
using ImagineCommunications.GamePlan.ReportSystem.Tests.ExcelTests.Helper;
using Xunit;

namespace ImagineCommunications.GamePlan.ReportSystem.Tests.ExcelTests
{
    public class BlockOptionsBuilderShould
    {
        private int BlockOptionsBuilderValue = 1;

        [Fact]
        public void CreateDefaultConfig()
        {
            var builder = CreateBlockOptionsBuilder();
            var opts = builder.Build();

            Assert.Equal(1, opts.Value);
        }

        [Fact]
        public void SetBackground()
        {
            var builder = CreateBlockOptionsBuilder();
            _ = builder.Background(Color.White);
            var option = builder.Build();

            Assert.NotNull(option.BackgroundColor);
            Assert.Equal(Color.White, option.BackgroundColor.Value);
        }

        [Fact]
        public void SetColSpan()
        {
            var colSpan = 2;
            var builder = CreateBlockOptionsBuilder();
            _ = builder.ColSpan(colSpan);
            var option = builder.Build();

            Assert.Equal(colSpan, option.ColSpan);
        }

        [Fact]
        public void DefaultColSpan()
        {
            var builder = CreateBlockOptionsBuilder();
            var option = builder.Build();

            Assert.Equal(1, option.ColSpan);
        }

        [Fact]
        public void SetHAlign()
        {
            var builder = CreateBlockOptionsBuilder();
            _ = builder.HAlign(ExcelHorizontalAlignment.Center);
            var option = builder.Build();

            Assert.Equal(ExcelHorizontalAlignment.Center, option.HAlign);
        }

        [Fact]
        public void SetVAlign()
        {
            var builder = CreateBlockOptionsBuilder();
            _ = builder.VAlign(ExcelVerticalAlignment.Center);
            var option = builder.Build();

            Assert.Equal(ExcelVerticalAlignment.Center, option.VAlign);
        }

        [Fact]
        public void SetStyle()
        {
            var builder = CreateBlockOptionsBuilder();
            _ = builder.Style(GamePlanReportStyles.HeaderStyle.Name);
            var option = builder.Build();

            Assert.Equal(GamePlanReportStyles.HeaderStyle.Name, option.StyleName);
        }

        private BlockOptionsBuilder<int> CreateBlockOptionsBuilder()
        {
            return new BlockOptionsBuilder<int>(BlockOptionsBuilderValue);
        }
    }
}
