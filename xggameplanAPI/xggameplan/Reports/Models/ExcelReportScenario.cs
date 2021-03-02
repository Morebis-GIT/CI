namespace xggameplan.Reports.Models
{
    public class ExcelReportScenario
    {
        public string Name { get; set; }
        public int MaxColumnsCount { get; set; }
        public ExcelReportGrid ScenarioDetails { get; set; }
        public ExcelReportGrid SalesAreaPassPriorities { get; set; }
        public ExcelReportGrid General { get; set; }
        public ExcelReportGrid Weighting { get; set; }
        public ExcelReportGrid Tolerance { get; set; }
        public ExcelReportGrid Rules { get; set; }
        public ExcelReportGrid ProgrammeRepetitions { get; set; }
        public ExcelReportGrid MinRatingPoints { get; set; }
        public ExcelReportGrid BreakExclusions { get; set; }
        public ExcelReportGrid SlottingLimits { get; set; }
    }
}
