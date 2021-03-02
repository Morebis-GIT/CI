using System.Collections.Generic;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.OneTable.EntityConfig;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel.SheetBuilder.OneTable
{
    public interface IOneTableSheetBuilder : IBaseSheetBuilder<OneTableSheetBuilder>
    {
        void DataContent<T>(IEnumerable<T> data, IOneTableExcelReportConfiguration configuration)
            where T : class;
        void DataContent<T>(IEnumerable<T> data)
            where T : class;
    }
}
