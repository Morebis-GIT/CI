using System.Drawing;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles.Enums;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel.SheetBuilder.BlockBuilder
{
    public class BlockOptions<T>: IBlockOptions
    {
        public new T Value { get; set; }
        public int ColSpan { get; set; } = 1;
        public int RowSpan { get; set; } = 1;
        public Color? BackgroundColor { get; set; }
        public ExcelHorizontalAlignment? HAlign { get; set; }
        public ExcelVerticalAlignment? VAlign { get; set; }
        public ExcelReportConfiguration.Styles.ExcelStyle Style { get; set; }
        public DisplayType Display { get; set; } = DisplayType.Inline;
        public string StyleName { get; set; }
        object IBlockOptions.Value => Value;
    }
}
