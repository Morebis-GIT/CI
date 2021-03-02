using System.Drawing;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles.Enums;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel.SheetBuilder.BlockBuilder
{
    public interface IBlockOptionsBuilder
    {
        IBlockOptionsBuilder ColSpan(int colSpan);
        IBlockOptionsBuilder RowSpan(int rowSpan);
        IBlockOptionsBuilder Background(Color color);
        IBlockOptionsBuilder HAlign(ExcelHorizontalAlignment alignment);
        IBlockOptionsBuilder VAlign(ExcelVerticalAlignment alignment);
        IBlockOptionsBuilder Style(ExcelReportConfiguration.Styles.ExcelStyle style);
        IBlockOptionsBuilder Style(string styleName);
        IBlockOptions Build();
    }
}
