using System.Collections.Generic;
using System.Drawing;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles.Enums;

namespace xggameplan.core.ReportGenerators
{
    public static class GamePlanReportStyles
    {
        public static ExcelPredefineStyle DataCellStyle =>
            new ExcelPredefineStyle("Data Cell", new ExcelStyle()
            {
                Border = new ExcelBorder(new ExcelBorderItem
                {
                    Style = ExcelBorderStyle.Hair
                }),
                Font = new ExcelFont
                {
                    FontColor = Color.Black,
                    Font = new Font("Calibri", 11, FontStyle.Regular)
                },
                Fill = new ExcelFill
                {
                    PatternType = ExcelFillStyle.Solid,
                    BackgroundColor = Color.White
                }
            });

        public static ExcelPredefineStyle EmptyCellStyle =>
           new ExcelPredefineStyle("Empty Cell", new ExcelStyle()
           {
               Border = new ExcelBorder(new ExcelBorderItem
               {
                   Style = ExcelBorderStyle.Hair
               }),
               Font = new ExcelFont
               {
                   FontColor = Color.Black,
                   Font = new Font("Calibri", 11, FontStyle.Regular)
               },
               Fill = new ExcelFill
               {
                   PatternType = ExcelFillStyle.LightDown,
                   PatternColor = Color.LightGray,
                   BackgroundColor = Color.White
               }
           });

        public static ExcelPredefineStyle CourierNewFontCellStyle =>
           new ExcelPredefineStyle("Break Exclusions", new ExcelStyle()
           {
               Border = new ExcelBorder(new ExcelBorderItem
               {
                   Style = ExcelBorderStyle.Hair
               }),
               Font = new ExcelFont
               {
                   FontColor = Color.Black,
                   Font = new Font("Courier New", 11, FontStyle.Regular)
               },
               Fill = new ExcelFill
               {
                   PatternType = ExcelFillStyle.Solid,
                   BackgroundColor = Color.White
               }
           });

        public static ExcelPredefineStyle HeaderStyle =>
            new ExcelPredefineStyle("Header 1", new ExcelStyle
            {
                Border = new ExcelBorder(new ExcelBorderItem
                {
                    Style = ExcelBorderStyle.Thin
                }),
                Font = new ExcelFont
                {
                    FontColor = Color.White,
                    Font = new Font("Calibri", 11, FontStyle.Bold)
                },
                Fill = new ExcelFill
                {
                    PatternType = ExcelFillStyle.Solid,
                    BackgroundColor = Color.FromArgb(31, 78, 120)
                }
            });

        public static ExcelPredefineStyle LightHeaderStyle =>
           new ExcelPredefineStyle("Header 2", new ExcelStyle
           {
               Border = new ExcelBorder(new ExcelBorderItem
               {
                   Style = ExcelBorderStyle.Thin
               }),
               Font = new ExcelFont
               {
                   FontColor = Color.White,
                   Font = new Font("Calibri", 11, FontStyle.Bold)
               },
               Fill = new ExcelFill
               {
                   PatternType = ExcelFillStyle.Solid,
                   BackgroundColor = Color.FromArgb(47, 117, 181)
               }
           });

        public static IEnumerable<ExcelPredefineStyle> AllPredefineStyles
        {
            get { return new[] { HeaderStyle, LightHeaderStyle, DataCellStyle, CourierNewFontCellStyle, EmptyCellStyle }; }
        }

        public static IEnumerable<ExcelPredefineStyle> RunReportPredefineStyles
        {
            get { return new[] { HeaderStyle, LightHeaderStyle, DataCellStyle, EmptyCellStyle, CourierNewFontCellStyle }; }
        }

        public static IEnumerable<ExcelPredefineStyle> SmoothFailuresReportPredefineStyles
        {
            get { return new[] { HeaderStyle, LightHeaderStyle, DataCellStyle, EmptyCellStyle }; }
        }

        public static IEnumerable<ExcelPredefineStyle> RecommendationReportPredefineStyles
        {
            get { return new[] { HeaderStyle, LightHeaderStyle, DataCellStyle, EmptyCellStyle }; }
        }

        public static Color[] AlternateBackgroundColors => new[] {Color.FromArgb(0xF5, 0xF5, 0xF5), Color.Transparent};
    }

}
