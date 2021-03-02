using System.Collections.Generic;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.EntityConfig;
using OfficeOpenXml;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel.SheetBuilder
{
    public class BaseSheetBuilder<TSheetBuilder>
        : IBaseSheetBuilder<TSheetBuilder>
        where TSheetBuilder : BaseSheetBuilder<TSheetBuilder>
    {
        protected IExcelStyleApplier StyleApplier { get; set; }
        protected ExcelWorksheet Worksheet { get; set; }
        protected int InitColumnNumber { get; set; } = 1;
        protected Cursor Cursor { get; set; }

        protected BaseSheetBuilder(IExcelStyleApplier styleApplier, ExcelWorksheet worksheet)
        {
            StyleApplier = styleApplier;
            Worksheet = worksheet;
            Cursor = new Cursor { CurrentColumn = InitColumnNumber };
        }

        public TSheetBuilder Freeze(int freezeFirstRows, int freezeFirstColumns)
        {
            Worksheet.View.FreezePanes(freezeFirstColumns, freezeFirstRows);
            return (TSheetBuilder)this;
        }

        public TSheetBuilder StartColumn(int startColumn)
        {
            InitColumnNumber = startColumn;
            return (TSheetBuilder)this;
        }

        public TSheetBuilder Skip(int rowsCount = 1)
        {
            Cursor.CurrentRow += rowsCount;
            return (TSheetBuilder)this;
        }

        protected virtual TSheetBuilder DataContentInternal<T>(IEnumerable<T> data, IExcelReportConfiguration configuration)
            where T : class
        {
            var writer = new ExcelDataWriter(Worksheet, StyleApplier, Cursor, InitColumnNumber);
            writer.Write(data, configuration);
            return (TSheetBuilder)this;
        }

        protected virtual TSheetBuilder DataContentInternal<T>(IEnumerable<T> data)
            where T : class
        {
            var config = CreateDefaultConfiguration<T>();
            return DataContentInternal(data, config);
        }

        protected virtual IExcelReportConfiguration CreateDefaultConfiguration<T>()
            where T : class
        {
            var configurationBuilder = new ExcelConfigurationBuilder<T>();
            return configurationBuilder.BuildConfiguration();
        }
    }
}
