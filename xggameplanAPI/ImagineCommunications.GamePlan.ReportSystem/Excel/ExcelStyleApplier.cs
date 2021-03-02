using OfficeOpenXml;
using OfficeOpenXml.Style;
using ExcelBorderItem = ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles.ExcelBorderItem;
using ExcelStyle = ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles.ExcelStyle;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel
{
    public class ExcelStyleApplier: IExcelStyleApplier
    {
        public virtual void ApplyStyle(string styleName, ExcelStyle style, ExcelRange range)
        {
            if(string.IsNullOrWhiteSpace(styleName) && style is null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(styleName))
            {
                ApplyStyle(style, range.Style);
            }
            else
            {
                range.StyleName = styleName;
            }
        }
        public virtual void ApplyStyle(ExcelStyle sourceStyle, OfficeOpenXml.Style.ExcelStyle destinationStyle)
        {
            if (sourceStyle == null)
            {
                return;
            }

            destinationStyle.WrapText = sourceStyle.WrapText;
            if (!string.IsNullOrWhiteSpace(sourceStyle.NumberFormat))
            {
                destinationStyle.Numberformat.Format = sourceStyle.NumberFormat;
            }

            ApplyBorder(sourceStyle, destinationStyle);
            ApplyFill(sourceStyle, destinationStyle);
            ApplyFont(sourceStyle, destinationStyle);
            ApplyAlignment(sourceStyle, destinationStyle);
        }

        protected virtual void ApplyAlignment(ExcelStyle sourceStyle, OfficeOpenXml.Style.ExcelStyle destinationStyle)
        {
            if (sourceStyle.HorizontalAlignment.HasValue)
            {
                destinationStyle.HorizontalAlignment = (ExcelHorizontalAlignment)sourceStyle.HorizontalAlignment.Value;
            }

            if (sourceStyle.VerticalAlignment.HasValue)
            {
                destinationStyle.VerticalAlignment = (ExcelVerticalAlignment)sourceStyle.VerticalAlignment.Value;
            }
        }

        protected virtual void ApplyFont(ExcelStyle sourceStyle, OfficeOpenXml.Style.ExcelStyle destinationStyle)
        {
            if (sourceStyle.Font == null)
            {
                return;
            }

            if (sourceStyle.Font.Font != null)
            {
                destinationStyle.Font.SetFromFont(sourceStyle.Font.Font);
            }

            if (sourceStyle.Font.FontColor.HasValue)
            {
                destinationStyle.Font.Color.SetColor(sourceStyle.Font.FontColor.Value);
            }

            if (sourceStyle.Font.VerticalAlignmentFont.HasValue)
            {
                destinationStyle.Font.VerticalAlign = (ExcelVerticalAlignmentFont)sourceStyle.Font.VerticalAlignmentFont.Value;
            }
        }

        protected virtual void ApplyFill(ExcelStyle sourceStyle, OfficeOpenXml.Style.ExcelStyle destinationStyle)
        {
            if (sourceStyle.Fill == null)
            {
                return;
            }

            if (sourceStyle.Fill.PatternType.HasValue)
            {
                destinationStyle.Fill.PatternType = (ExcelFillStyle)sourceStyle.Fill.PatternType;
            }

            if (sourceStyle.Fill.BackgroundColor != null)
            {
                destinationStyle.Fill.BackgroundColor.SetColor(sourceStyle.Fill.BackgroundColor.Value);
            }

            if (sourceStyle.Fill.PatternColor != null)
            {
                destinationStyle.Fill.PatternColor.SetColor(sourceStyle.Fill.PatternColor.Value);
            }
        }

        protected virtual void ApplyBorder(ExcelStyle sourceStyle, OfficeOpenXml.Style.ExcelStyle destinationStyle)
        {
            if (sourceStyle.Border == null)
            {
                return;
            }

            ApplyBorderItem(destinationStyle.Border.Bottom, sourceStyle.Border.Bottom);
            ApplyBorderItem(destinationStyle.Border.Top, sourceStyle.Border.Top);
            ApplyBorderItem(destinationStyle.Border.Right, sourceStyle.Border.Right);
            ApplyBorderItem(destinationStyle.Border.Left, sourceStyle.Border.Left);
        }

        protected virtual void ApplyBorderItem(OfficeOpenXml.Style.ExcelBorderItem destinationItem, ExcelBorderItem sourceItem)
        {
            if (destinationItem == null || sourceItem == null)
            {
                return;
            }

            if (sourceItem.Color.HasValue)
            {
                destinationItem.Color.SetColor(sourceItem.Color.Value);
            }

            if (sourceItem.Style.HasValue)
            {
                destinationItem.Style = (ExcelBorderStyle)sourceItem.Style;
            }
        }
    }
}
