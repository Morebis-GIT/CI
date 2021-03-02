using System.Collections.Generic;

namespace xggameplan.Reports.Models
{
    public class ExcelReportRunModel
    {
        public List<ExcelReportScenario> Scenarios { get; set; }
        public ExcelReportRunModel()
        {
            Scenarios = new List<ExcelReportScenario>();
        }
    }
}
