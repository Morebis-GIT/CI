using System.Collections.Generic;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.EntityConfig;
using ImagineCommunications.GamePlan.ReportSystem.Excel.SheetBuilder;
using xggameplan.Model;

namespace xggameplan.Reports.Common
{
    public interface IReportColumnFormatter
    {
        List<T> ApplySettings<T>(IEnumerable<T> data, IEnumerable<ColumnStatusModel> columnStatusModelList,
            ExcelConfigurationBuilder<T> configurationBuilder) where T : class;

        void AutoFitAll(ISheetBuilder sheetBuilder);

        void AutoFit(List<int> columns, ISheetBuilder sheetBuilder);
    }
}
