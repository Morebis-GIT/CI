using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.EntityConfig;
using ImagineCommunications.GamePlan.ReportSystem.Excel.SheetBuilder.BlockBuilder;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel.SheetBuilder
{
    public interface ISheetBuilder: IBaseSheetBuilder<SheetBuilder>
    {
        ISheetBuilder Block(Action<IBlockBuilder> action);
        ISheetBuilder Column(int columnNumber, Action<IColumnBuilder> action);
        ISheetBuilder AutoFitColumns(int fromColumnsNumber, int toColumnsNumber);
        ISheetBuilder DataContent<T>(IEnumerable<T> data, IExcelReportConfiguration configuration)
            where T : class;

        ISheetBuilder DataContent<T>(IEnumerable<T> data)
            where T : class;
    }
}
