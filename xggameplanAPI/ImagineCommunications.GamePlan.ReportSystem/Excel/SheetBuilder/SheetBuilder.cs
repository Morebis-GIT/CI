using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.EntityConfig;
using ImagineCommunications.GamePlan.ReportSystem.Excel.SheetBuilder.BlockBuilder;
using OfficeOpenXml;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel.SheetBuilder
{
    public class SheetBuilder : BaseSheetBuilder<SheetBuilder>, ISheetBuilder
    {
        public SheetBuilder(IExcelStyleApplier styleApplier, ExcelWorksheet worksheet) : base(styleApplier, worksheet)
        {
        }

        public ISheetBuilder Block(Action<IBlockBuilder> action)
        {
            Cursor.CurrentColumn = InitColumnNumber;
            var initRowNumber = Cursor.CurrentRow;

            var block = new BlockBuilder.BlockBuilder(Worksheet);
            action(block);
            var writer = new ExcelDataWriter(Worksheet, StyleApplier, Cursor, InitColumnNumber);
            block.Write(writer);
            Cursor.CurrentRow = initRowNumber + block.GetRowCount();
            Cursor.CurrentColumn = InitColumnNumber;
            return this;
        }

        public ISheetBuilder Column(int columnNumber, Action<IColumnBuilder> action)
        {
            var column = Worksheet.Column(columnNumber);
            var columnBuilder = new ColumnBuilder(column);
            action(columnBuilder);
            return this;
        }

        public virtual ISheetBuilder AutoFitColumns(int fromColumnsNumber, int toColumnsNumber)
        {
            for (var i = fromColumnsNumber; i <= toColumnsNumber; i++)
            {
                Column(i, columnBuilder => columnBuilder.AutoFitColumn());
            }
            return this;
        }

        public virtual ISheetBuilder DataContent<T>(IEnumerable<T> data, IExcelReportConfiguration configuration)
            where T : class
        {
            return base.DataContentInternal(data, configuration);
        }

        public virtual ISheetBuilder DataContent<T>(IEnumerable<T> data)
            where T : class
        {
            return base.DataContentInternal(data);
        }

    }
}
