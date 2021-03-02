using ImagineCommunications.GamePlan.ReportSystem.Excel.SheetBuilder;
using OfficeOpenXml;
using Xunit;

namespace ImagineCommunications.GamePlan.ReportSystem.Tests.ExcelTests
{
    public class ExcelColumnBuilderShould
    {
        [Fact]
        public void SetWidth()
        {
            var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("test");
            var cb = new ColumnBuilder(worksheet.Column(1));

            _ = cb.Width(100);

            Assert.Equal(100, worksheet.Column(1).Width);
        }
    }
}
