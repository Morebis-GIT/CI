using xggameplan.Reports.Models;

namespace xggameplan.Reports
{
    public interface IExcelReportGenerator
    {
        byte[] GetRunExcelReport(ExcelReportRunModel run);

        byte[] GetSmoothFailuresExcelReport(ExcelReportSmoothFailuresModel reportModel);
    }
}
