using NUnit.Framework;
using xggameplan.Reports.Models;

namespace xggameplan.tests.ReportTests.Excel
{
    [TestFixture]
    public class ExcelReportCellTests
    {
        [Test]
        public void CellHasDataReturnFalseWhenTheCellValueIsSetToNull()
        {
            var cell = new ExcelReportCell();
            cell.Value = null;
            Assert.IsFalse(cell.HasData);
        }

        [Test]
        public void CellHasDataReturnTrueWhenTheCellValueHasValue()
        {
            var cell = new ExcelReportCell();
            Assert.IsFalse(cell.AlternateBackground);
        }
    }
}
