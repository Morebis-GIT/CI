namespace ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles
{
    public class ExcelBorder
    {
        public ExcelBorderItem Top { get; set; }
        public ExcelBorderItem Right { get; set; }
        public ExcelBorderItem Bottom { get; set; }
        public ExcelBorderItem Left { get; set; }

        public ExcelBorder()
        {
        }

        public ExcelBorder(ExcelBorderItem borders): this(borders, borders, borders, borders)
        {

        }

        public ExcelBorder(ExcelBorderItem top, ExcelBorderItem right, ExcelBorderItem bottom, ExcelBorderItem left)
        {
            Top = top;
            Right = right;
            Bottom = bottom;
            Left = left;
        }
    }
}
