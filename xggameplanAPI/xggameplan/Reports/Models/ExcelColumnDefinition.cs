using System;

namespace xggameplan.Reports.Models
{
    public class ExcelColumnDefinition
    {
        public string PropertyName { get; set; }
        public string DisplayTitle { get; set; }
        public double Width { get; set; }
        public bool Ignore { get; set; }
        public Func<object, string> FormatExpression { get; set; }

        public ExcelColumnDefinition(string propertyName, double width)
        {
            this.PropertyName = propertyName;
            this.Width = width;
        }
    }
}
