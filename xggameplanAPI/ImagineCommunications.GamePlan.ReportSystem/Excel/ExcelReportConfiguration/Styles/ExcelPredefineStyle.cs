namespace ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles
{
    public class ExcelPredefineStyle
    {
        public string Name { get; set; }
        public ExcelStyle Style { get; set; }

        public ExcelPredefineStyle(string name)
        {
            Name = name;
        }

        public ExcelPredefineStyle(string name, ExcelStyle style): this(name)
        {
            Style = style;
        }
    }
}
