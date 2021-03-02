using System;
using System.Collections.Generic;
using AutoMapper;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using ImagineCommunications.GamePlan.Domain.SmoothFailureMessages;
using xggameplan.Model;
using xggameplan.Reports.Models;

namespace xggameplan.Reports.DataAdapters
{
    public interface IRunExcelReportDataAdapter
    {
        ExcelReportRunModel Map(RunModel run, IEnumerable<Demographic> demographics, DateTime reportDate);
        ExcelReportSmoothFailuresModel Map(Run run, List<SmoothFailureModel> smoothFailuresModel,
            IEnumerable<SmoothFailureMessage> failureMessages, DateTime reportDate, IMapper mapper);
    }
}
