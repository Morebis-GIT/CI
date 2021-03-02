using NUnit.Framework;
using xggameplan.Reports.Models;

namespace xggameplan.tests.ReportTests.Excel
{
    [TestFixture]
    public class ExcelReportRunModelTests
    {
        [Test]
        public void ScenariosAreInitWhenModelIsCreated()
        {
            var model = new ExcelReportRunModel();
            Assert.IsNotNull(model.Scenarios);
            Assert.AreEqual(0, model.Scenarios.Count);
        }
    }
}
