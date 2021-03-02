using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles.Enums;

namespace xggameplan.Reports.Models
{
    public class ExcelReportCell
    {
        public object Value { get; set; }
        public ExcelHorizontalAlignment Alignment { get; set; }
        public string StyleName { get; set; }
        public bool AlternateBackground { get; set; }

        public bool HasData
        {
            get
            {
                return Value != null;
            }
        }

    }
}
