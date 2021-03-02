using OfficeOpenXml;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel.SheetBuilder
{
    public class ColumnBuilder: IColumnBuilder
    {
        protected ExcelColumn Column { get; set; }
        public ColumnBuilder(ExcelColumn column)
        {
            Column = column;
        }

        public IColumnBuilder Width(double width)
        {
            Column.Width = width;
            return this;
        }

        public IColumnBuilder AutoFitColumn()
        {
            Column.AutoFit();
            return this;
        }

        public IColumnBuilder Format(string format)
        {
            Column.Style.Numberformat.Format = format;
            return this;
        }
    }
}
