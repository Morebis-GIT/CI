using System.Collections.Generic;
using System.Drawing;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles.Enums;

namespace ImagineCommunications.GamePlan.ReportSystem.Tests.ExcelTests.Helper
{
    public static class GamePlanReportStyles
    {
        public static ExcelPredefineStyle DataCellStyle =>
            new ExcelPredefineStyle("DataCell", new ExcelStyle()
            {
                Border = new ExcelBorder(new ExcelBorderItem
                {
                    Style = ExcelBorderStyle.Thin
                }),
                Font = new ExcelFont
                {
                    FontColor = Color.Black,
                    Font = new Font(FontFamily.GenericSansSerif, 11, FontStyle.Regular)
                },
                Fill = new ExcelFill
                {
                    PatternType = ExcelFillStyle.Solid,
                    BackgroundColor = Color.White
                }
            });

        public static ExcelPredefineStyle HeaderStyle =>
            new ExcelPredefineStyle("Header", new ExcelStyle
            {
                Border = new ExcelBorder(new ExcelBorderItem
                {
                    Style = ExcelBorderStyle.Thin
                }),
                Font = new ExcelFont
                {
                    FontColor = Color.White,
                    Font = new Font(FontFamily.GenericSansSerif, 11, FontStyle.Bold)
                },
                Fill = new ExcelFill
                {
                    PatternType = ExcelFillStyle.Solid,
                    BackgroundColor = Color.FromArgb(31, 78, 120)
                }
            });

        public static ExcelPredefineStyle LightBlueFontColorCellStyle =>
            new ExcelPredefineStyle("LightBlueFontColorCell", new ExcelStyle
            {
                Border = new ExcelBorder(new ExcelBorderItem
                {
                    Style = ExcelBorderStyle.Thin
                }),
                Font = new ExcelFont
                {
                    FontColor = Color.LightBlue,
                    Font = new Font(FontFamily.GenericSansSerif, 11, FontStyle.Regular)
                },
                Fill = new ExcelFill
                {
                    PatternType = ExcelFillStyle.Solid,
                    BackgroundColor = Color.White
                }
            });

        public static ExcelPredefineStyle LightHeaderStyle =>
            new ExcelPredefineStyle("LightHeader", new ExcelStyle
            {
                Border = new ExcelBorder(new ExcelBorderItem
                {
                    Style = ExcelBorderStyle.Thin
                }),
                Font = new ExcelFont
                {
                    FontColor = Color.White,
                    Font = new Font(FontFamily.GenericSansSerif, 11, FontStyle.Bold)
                },
                Fill = new ExcelFill
                {
                    PatternType = ExcelFillStyle.Solid,
                    BackgroundColor = Color.FromArgb(47, 117, 181)
                }
            });

        public static IEnumerable<ExcelPredefineStyle> GetAllPredefineStyles
        {
            get { return new[] {HeaderStyle, LightHeaderStyle, DataCellStyle, LightBlueFontColorCellStyle}; }
        }
    }
}
