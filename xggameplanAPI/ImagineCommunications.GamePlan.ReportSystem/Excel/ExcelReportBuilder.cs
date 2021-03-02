using System;
using ImagineCommunications.GamePlan.ReportSystem.Excel.SheetBuilder;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel
{
    public class ExcelReportBuilder: BaseExcelReportBuilder<ISheetBuilder>
        , IExcelReportBuilder
    {
        public ExcelReportBuilder(IExcelStyleApplier styleApplier) : base(styleApplier)
        {
        }

        public override IBaseExcelReportBuilder<ISheetBuilder> Sheet(string sheetName, Action<ISheetBuilder> action)
        {
            var worksheet = Workbook.Worksheets.Add(sheetName);

            var sheetBuilder = new SheetBuilder.SheetBuilder(StyleApplier, worksheet);
            action(sheetBuilder);

            return this;
        }

        protected override void Dispose(bool disposing) => base.Dispose(disposing);
    }
}
