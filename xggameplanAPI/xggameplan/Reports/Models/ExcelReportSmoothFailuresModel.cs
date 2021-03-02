using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using xggameplan.Model;

namespace xggameplan.Reports.Models
{
    public class ExcelReportSmoothFailuresModel
    {
        public Run Run { get; set; }
        public List<SmoothFailureExtendedModel> SmoothFailures { get; set; }
        public DateTime ReportDate { get; set; }
    }
}
