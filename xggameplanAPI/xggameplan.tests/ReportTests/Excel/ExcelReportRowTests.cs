using NUnit.Framework;
using xggameplan.Reports.Models;

namespace xggameplan.tests.ReportTests.Excel
{
    [TestFixture]
    public class ExcelReportRowTests
    {
        [Test]
        public void CellsAreInitWhenRowIsCreated()
        {
            var row = new ExcelReportRow();
            Assert.IsNotNull(row.Cells);
            Assert.AreEqual(0, row.Cells.Count);
        }

        [Test]
        public void HasDataReturnFalseWhenNoneOfTheCellsOfTheGridHasAnyValue()
        {
            var row = new ExcelReportRow();
            row.Cells.Add(new ExcelReportCell());
            Assert.IsFalse(row.HasData);
        }

        [Test]
        public void HasDataReturnTrueWhenAtleastOneOfTheCellsNotInTheFirstColumnHasValue()
        {
            var row = new ExcelReportRow();
            row.Cells.Add(new ExcelReportCell());
            row.Cells.Add(new ExcelReportCell() { Value = 10 });
            Assert.IsTrue(row.HasData);
        }

        [Test]
        public void HasDataReturnFalseWhenOnlyFirstCellHasValue()
        {
            var row = new ExcelReportRow();
            row.Cells.Add(new ExcelReportCell() { Value = 10 });
            Assert.IsFalse(row.HasData);
        }
    }
}
