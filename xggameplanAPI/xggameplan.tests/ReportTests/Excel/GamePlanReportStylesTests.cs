using System.Drawing;
using System.Linq;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles.Enums;
using NUnit.Framework;
using xggameplan.core.ReportGenerators;

namespace xggameplan.tests.ReportTests.Excel
{
    [TestFixture]
    public class GamePlanReportStylesTests
    {
        [Test]
        public void TestDataCellStyleHasTheRightSettings()
        {
            var style = GamePlanReportStyles.DataCellStyle;
            Assert.AreEqual("Data Cell", style.Name);
            AssertBorder(ExcelBorderStyle.Hair, style.Style.Border);
            AssertFont("Calibri", 11, FontStyle.Regular, Color.Black, style.Style.Font);
            AssertFill(Color.White, ExcelFillStyle.Solid, style.Style.Fill);
        }

        [Test]
        public void TestEmptyCellStyleHasTheRightSettings()
        {
            var style = GamePlanReportStyles.EmptyCellStyle;
            Assert.AreEqual("Empty Cell", style.Name);
            AssertBorder(ExcelBorderStyle.Hair, style.Style.Border);
            AssertFont("Calibri", 11, FontStyle.Regular, Color.Black, style.Style.Font);
            AssertFill(Color.White, Color.LightGray, ExcelFillStyle.LightDown, style.Style.Fill);
        }

        [Test]
        public void TestCourierNewFontCellStyleHasTheRightSettings()
        {
            var style = GamePlanReportStyles.CourierNewFontCellStyle;
            Assert.AreEqual("Break Exclusions", style.Name);
            AssertBorder(ExcelBorderStyle.Hair, style.Style.Border);
            AssertFont("Courier New", 11, FontStyle.Regular, Color.Black, style.Style.Font);
            AssertFill(Color.White, ExcelFillStyle.Solid, style.Style.Fill);
        }

        [Test]
        public void TestHeaderStyleHasTheRightSettings()
        {
            var style = GamePlanReportStyles.HeaderStyle;
            Assert.AreEqual("Header 1", style.Name);
            AssertBorder(ExcelBorderStyle.Thin, style.Style.Border);
            AssertFont("Calibri", 11, FontStyle.Bold, Color.White, style.Style.Font);
            AssertFill(Color.FromArgb(31, 78, 120), ExcelFillStyle.Solid, style.Style.Fill);
        }

        [Test]
        public void TestLightHeaderStyleHasTheRightSettings()
        {
            var style = GamePlanReportStyles.LightHeaderStyle;
            Assert.AreEqual("Header 2", style.Name);
            AssertBorder(ExcelBorderStyle.Thin, style.Style.Border);
            AssertFont("Calibri", 11, FontStyle.Bold, Color.White, style.Style.Font);
            AssertFill(Color.FromArgb(47, 117, 181), ExcelFillStyle.Solid, style.Style.Fill);
        }

        [Test]
        public void RunReportPredefineStylesHasTheRightValues()
        {
            var runReportPredefineStyles = GamePlanReportStyles.RunReportPredefineStyles.ToList();
            Assert.AreEqual(5, runReportPredefineStyles.Count);
            Assert.AreEqual("Header 1", runReportPredefineStyles[0].Name);
            Assert.AreEqual("Header 2", runReportPredefineStyles[1].Name);
            Assert.AreEqual("Data Cell", runReportPredefineStyles[2].Name);
            Assert.AreEqual("Empty Cell", runReportPredefineStyles[3].Name);
            Assert.AreEqual("Break Exclusions", runReportPredefineStyles[4].Name);
        }

        private void AssertFill(Color expectedBackgroundColor, ExcelFillStyle expectedFillStyle, ExcelFill fill)
        {
            Assert.AreEqual(expectedBackgroundColor, fill.BackgroundColor);
            Assert.AreEqual(expectedFillStyle, fill.PatternType);
        }

        private void AssertFill(Color expectedBackgroundColor, Color expectedPatternColor, ExcelFillStyle expectedFillStyle, ExcelFill fill)
        {
            AssertFill(expectedBackgroundColor, expectedFillStyle, fill);
            Assert.AreEqual(expectedPatternColor, fill.PatternColor);
        }

        private void AssertFont(string expectedName, int expectedSize, FontStyle expectedStyle, Color expectedColor, ExcelFont font)
        {
            Assert.AreEqual(expectedName, font.Font.FontFamily.Name);
            Assert.AreEqual(expectedSize, font.Font.Size);
            Assert.AreEqual(expectedStyle, font.Font.Style);
            Assert.AreEqual(expectedColor, font.FontColor);
        }

        private void AssertBorder(ExcelBorderStyle expectedBorderStyle, ExcelBorder border)
        {
            Assert.AreEqual(expectedBorderStyle, border.Bottom.Style);
            Assert.AreEqual(expectedBorderStyle, border.Left.Style);
            Assert.AreEqual(expectedBorderStyle, border.Top.Style);
            Assert.AreEqual(expectedBorderStyle, border.Right.Style);
        }
    }
}
