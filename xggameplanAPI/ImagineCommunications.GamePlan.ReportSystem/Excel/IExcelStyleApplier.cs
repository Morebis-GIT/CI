using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles;
using OfficeOpenXml;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel
{
    public interface IExcelStyleApplier
    {
        void ApplyStyle(string styleName, ExcelStyle style, ExcelRange range);

        void ApplyStyle(ExcelStyle sourceStyle, OfficeOpenXml.Style.ExcelStyle destinationStyle);
    }
}
