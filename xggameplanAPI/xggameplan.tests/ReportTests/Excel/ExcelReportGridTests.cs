using NUnit.Framework;
using xggameplan.Reports.Models;

namespace xggameplan.tests.ReportTests.Excel
{
    [TestFixture]
    public class ExcelReportGridTests
    {
        [Test]
        public void HeaderRowsAreInitWhenGridIsCreated()
        {
            var grid = new ExcelReportGrid();
            Assert.IsNotNull(grid.HeaderRows);
            Assert.AreEqual(0,grid.HeaderRows.Count);
        }

        [Test]
        public void BodyRowsAreInitWhenGridIsCreated()
        {
            var grid = new ExcelReportGrid();
            Assert.IsNotNull(grid.BodyRows);
            Assert.AreEqual(0, grid.BodyRows.Count);
        }

        [Test]
        public void HasDataReturnFalseWhenNoneOfTheCellsOfTheGridHasAnyValue()
        {
            var grid = new ExcelReportGrid();
            var row1 = new ExcelReportRow();
            var row2 = new ExcelReportRow();
            row1.Cells.Add(new ExcelReportCell());
            row2.Cells.Add(new ExcelReportCell());
            grid.BodyRows.Add(row1);
            grid.BodyRows.Add(row2);
            Assert.IsFalse(grid.HasData);
        }

        [Test]
        public void HasDataReturnTrueWhenAtleastOneOfTheCellsNotInTheFirstColumnHasValue()
        {
            var grid = new ExcelReportGrid();
            var row1 = new ExcelReportRow();
            var row2 = new ExcelReportRow();
            row1.Cells.Add(new ExcelReportCell());
            row1.Cells.Add(new ExcelReportCell() { Value = 10});
            row2.Cells.Add(new ExcelReportCell());
            grid.BodyRows.Add(row1);
            grid.BodyRows.Add(row2);
            Assert.IsTrue(grid.HasData);
        }

        [Test]
        public void HasDataReturnFalseWhenOnlyFirstColumnHasValue()
        {
            var grid = new ExcelReportGrid();
            var row1 = new ExcelReportRow();
            var row2 = new ExcelReportRow();
            row1.Cells.Add(new ExcelReportCell() { Value = 10 });
            row2.Cells.Add(new ExcelReportCell());
            grid.BodyRows.Add(row1);
            grid.BodyRows.Add(row2);
            Assert.IsFalse(grid.HasData);
        }
    }
}
