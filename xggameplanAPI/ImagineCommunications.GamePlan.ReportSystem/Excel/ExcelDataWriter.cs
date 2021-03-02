using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.EntityConfig;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.MemberConfig;
using ImagineCommunications.GamePlan.ReportSystem.HeaderHumanizer;
using ImagineCommunications.GamePlan.ReportSystem.TypeAccessor;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ExcelStyle = ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.Styles.ExcelStyle;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel
{
    public class ExcelDataWriter: IExcelDataWriter
    {
        protected ExcelWorksheet Worksheet { get; set; }
        protected Cursor Cursor { get; }
        protected IExcelStyleApplier ExcelStyleApplier { get; set; }
        protected int StartColumn { get; set; }

        public ExcelDataWriter(ExcelWorksheet excelWorksheet, IExcelStyleApplier excelStyleApplier, Cursor cursor, int startColumn)
        {
            ExcelStyleApplier = excelStyleApplier;
            Cursor = cursor;
            Worksheet = excelWorksheet ?? throw new ArgumentNullException(nameof(excelWorksheet));
            StartColumn = startColumn;
        }

        public void WriteValue<T>(T value, IExcelMemberOptions options, DisplayType display)
        {
            WriteMember(value, options, display);
        }

        public void Write<T>(IEnumerable<T> source, IExcelReportConfiguration configuration)
            where T : class
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var accessor = new TypeAccessor<T>();

            Write(source, configuration, accessor);
        }

        private void Write(IEnumerable source, IExcelReportConfiguration config, TypeAccessor.TypeAccessor accessor)
        {
            if (config.Options.IsSetHeaderStyle)
            {
                var headerCells = GetHeaderRange(Worksheet, config, Cursor);
                ExcelStyleApplier.ApplyStyle(config.Options.HeaderStyleName, config.Options.HeaderStyle, headerCells);
            }

            WriteHeader(config);

            WriteRecords(source, config, accessor);
        }

        private static ExcelRange GetHeaderRange(ExcelWorksheet worksheet, IExcelReportConfiguration config, Cursor cursor)
        {
            var columnCount = 0;
            var memberOptions = config.MemberConfigurations.GetActiveOrderlyOptions();
            foreach (var column in memberOptions)

            {
                columnCount += column.ColSpan;
            }

            var endColumn = cursor.CurrentColumn + columnCount - 1;
            return worksheet.Cells[cursor.CurrentRow, cursor.CurrentColumn, cursor.CurrentRow, endColumn];
        }

        protected virtual void WriteRecords(IEnumerable source, IExcelReportConfiguration config, TypeAccessor.TypeAccessor accessor)
        {
            var memberOptions = config.MemberConfigurations.GetActiveOrderlyOptions().ToList();

            int countRows = CountRows(source);
            var cacheFormatterExpression = new Dictionary<string, Func<object, string>>();

            int excelRowsCount = memberOptions.Max(r => r.RowSpan) * countRows;
            int excelColSpanCount = memberOptions.Sum(r => r.ColSpan);
            var data = new object[excelRowsCount, excelColSpanCount];

            var insertedRowFromSource = -1;
            foreach (var row in source)
            {
                insertedRowFromSource++;
                var columnIndex = -1;
                foreach (var options in memberOptions)
                {
                    object value = accessor.GetValue(row, options.MemberName);

                    if (!options.IsStyleSet && config.Options.IsSetDefaultStyle)
                    {
                        options.IsStyleSet = config.Options.IsSetDefaultStyle;
                        options.StyleName = config.Options.DefaultStyleName;
                    }

                    string formattedValue = null;
                    if (options.Formatter != null)
                    {
                        formattedValue = options.Formatter.Format(value);
                    }
                    else if (options.FormatterExpression != null)
                    {
                        if (!cacheFormatterExpression.ContainsKey(options.MemberName))
                        {
                            cacheFormatterExpression[options.MemberName] =
                                ReportHelper.ReportHelper.CreateFormatFunc(value, options.FormatterExpression);
                        }

                        formattedValue =
                            cacheFormatterExpression[options.MemberName](value);
                    }

                    for (var rowIndex = 0; rowIndex < options.RowSpan; rowIndex++)
                    {
                        for (var colIndex = 0; colIndex < options.ColSpan; colIndex++)
                        {
                            columnIndex++;
                            data[insertedRowFromSource + rowIndex, columnIndex] = formattedValue ?? value;
                        }
                    }
                }
            }

            var prevCursor = (Cursor)Cursor.Clone();
            WriteData(data);
            ApplyStyleForWrittenData(prevCursor, config, memberOptions, data);
        }

        private void WriteData(object[,] data)
        {
            var lastMemberRow = GetLastMemberPosition(Cursor.CurrentRow, data.GetLength(0));
            var lastMemberColumn = GetLastMemberPosition(Cursor.CurrentColumn, data.GetLength(1));
            var range = Worksheet.Cells[Cursor.CurrentRow, Cursor.CurrentColumn, lastMemberRow, lastMemberColumn];

            range.Value = data;

            Cursor.CurrentRow++;
            Cursor.CurrentColumn = StartColumn;
        }

        private void ApplyStyleForWrittenData(Cursor prevCursor, IExcelReportConfiguration config, IReadOnlyCollection<IExcelMemberOptions> memberOptions, object[,] data)
        {
            var alternativeCursor = (Cursor)prevCursor.Clone();
            var rowsCount = data.GetLength(0);
            if (rowsCount == 0)
            {
                return;
            }

            var maxRow = GetLastMemberPosition(alternativeCursor.CurrentRow, rowsCount);
            var maxColumn = data.GetLength(1);
            var allRange = Worksheet.Cells[2, 1, maxRow, maxColumn];

            // set default style for all cells for performance reasons
            var optionsWithStyle = memberOptions.FirstOrDefault(x => x.IsStyleSet);
            if (optionsWithStyle != null)
            {
                ApplyStylesForRange(allRange, optionsWithStyle);
            }

            // set default background color for all cells for performance reasons
            if (config.Options.HasAlternateBackgroundColor && config.Options.AlternateBackgroundColors?.Count > 0)
            {
                if (optionsWithStyle is null || allRange.Style.Fill.PatternType != ExcelFillStyle.Solid)
                {
                    allRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                }

                allRange.Style.Fill.BackgroundColor.SetColor(config.Options.AlternateBackgroundColors[0].Value);
            }

            var currentMemberColumn = prevCursor.CurrentColumn;
            foreach (var options in memberOptions)
            {
                var lastMemberRow = GetLastMemberPosition(prevCursor.CurrentRow, rowsCount);
                var lastMemberColumn = GetLastMemberPosition(currentMemberColumn, options.ColSpan);
                var columnRange = allRange[prevCursor.CurrentRow, currentMemberColumn, lastMemberRow, lastMemberColumn];

                if (!string.IsNullOrWhiteSpace(options.FormatNumber))
                {
                    columnRange.Style.Numberformat.Format = options.FormatNumber;
                }

                object value = data[0, currentMemberColumn - 1];

                if ((value is DateTime || value is DateTimeOffset)
                    && IsDefaultFormatNumber(columnRange)
                    && DateTimeFormatInfo.CurrentInfo != null &&
                    columnRange.Style.Numberformat.Format != DateTimeFormatInfo.CurrentInfo.ShortDatePattern)
                {
                    columnRange.Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                }

                ApplyStylesForRange(columnRange, options);

                currentMemberColumn = lastMemberColumn + 1;
            }

            if (config.Options.HasAlternateBackgroundColor)
            {
                ApplyAlternativeBackgroundColors(config.Options.AlternateBackgroundColors, allRange, maxRow, maxColumn);
            }
        }

        private static void ApplyStylesForRange(ExcelRange range, IExcelMemberOptions options)
        {
            if (options.HAlign.HasValue && range.Style.HorizontalAlignment != (ExcelHorizontalAlignment)options.HAlign)
            {
                range.Style.HorizontalAlignment = (ExcelHorizontalAlignment)options.HAlign;
            }

            if (options.VAlign.HasValue && range.Style.VerticalAlignment != (ExcelVerticalAlignment)options.VAlign)
            {
                range.Style.VerticalAlignment = (ExcelVerticalAlignment)options.VAlign;
            }

            if (options.BackgroundColor.HasValue && range.Style.Fill.BackgroundColor.Rgb != options.BackgroundColor.Value.ToArgb().ToString("X"))
            {
                range.Style.Fill.BackgroundColor.SetColor(options.BackgroundColor.Value);
            }

            if (options.IsStyleSet && range.StyleName != options.StyleName)
            {
                range.StyleName = options.StyleName;
            }
        }

        private static void ApplyAlternativeBackgroundColors(IReadOnlyList<KeyValuePair<string, Color>> alternateBackgroundColors, ExcelRange allRange, int maxRow, int maxColumn)
        {
            if (alternateBackgroundColors is null || alternateBackgroundColors.Count == 0)
            {
                return;
            }

            for (var r = 2; r <= maxRow; r++)
            {
                var inneRange = allRange[r, 1, r, maxColumn];
                var alternativeColor = alternateBackgroundColors[r % alternateBackgroundColors.Count];

                if (inneRange.Style.Fill.PatternType != ExcelFillStyle.Solid)
                {
                    inneRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                }

                if (inneRange.Style.Fill.BackgroundColor.Rgb != alternativeColor.Key)
                {
                    inneRange.Style.Fill.BackgroundColor.SetColor(alternativeColor.Value);
                }
            }
        }

        private static int CountRows(IEnumerable source)
        {
            if (source is ICollection collection)
            {
                return collection.Count;
            }

            var count = 0;
            foreach (var el in source)
            {
                count++;
            }

            return count;
        }

        protected virtual void WriteHeader(IExcelReportConfiguration config)
        {
            if (config.Options.IsHideHeader)
            {
                return;
            }

            var memberOptions = config.MemberConfigurations.GetActiveOrderlyOptions();
            foreach (var options in memberOptions)
            {
                string header = options.GetHeader(config.Options.UseHeaderHumanizer);

                if (!options.IsHeaderStyleSet && !config.Options.IsSetHeaderStyle && config.Options.IsSetDefaultStyle)
                {
                    options.HeaderStyle = config.Options.DefaultStyle;
                    options.HeaderStyleName = config.Options.DefaultStyleName;
                }

                WriteMember(header, options, DisplayType.Inline, true);
            }

            Cursor.CurrentRow++;
            Cursor.CurrentColumn = StartColumn;
        }

        private void WriteMember<T>(T value, IExcelMemberOptions options, DisplayType display, bool isHeaderRow = false)
        {
            var cursor = Cursor;
            var lastMemberColumn = GetLastMemberPosition(cursor.CurrentColumn, options.ColSpan);
            var lastMemberRow = GetLastMemberPosition(cursor.CurrentRow, options.RowSpan);

            var range = Worksheet.Cells[cursor.CurrentRow, cursor.CurrentColumn, lastMemberRow, lastMemberColumn];
            ApplyStyle(options, isHeaderRow, range);

            range.Value = value;
            range.Merge = options.Merge;

            if (!string.IsNullOrWhiteSpace(options.FormatNumber))
            {
                range.Style.Numberformat.Format = options.FormatNumber;
            }

            if ((value is DateTime || value is DateTimeOffset)
                && IsDefaultFormatNumber(range)
                && DateTimeFormatInfo.CurrentInfo != null)
            {
                range.Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
            }

            if (options.HAlign.HasValue)
            {
                range.Style.HorizontalAlignment = (ExcelHorizontalAlignment)options.HAlign;
            }

            if (options.VAlign.HasValue)
            {
                range.Style.VerticalAlignment = (ExcelVerticalAlignment)options.VAlign;
            }

            if (options.BackgroundColor.HasValue)
            {
                range.Style.Fill.BackgroundColor.SetColor(options.BackgroundColor.Value);
            }

            if (options.HasAlternateBackgroundColor && options.AlternateBackgroundColors != null
                && options.AlternateBackgroundColors.Length > 0 && !isHeaderRow)
            {
                range.Style.Fill.BackgroundColor.SetColor(options.AlternateBackgroundColors[(cursor.CurrentRow + 1) % options.AlternateBackgroundColors.Length]);
            }

            if (display == DisplayType.Block)
            {
                cursor.CurrentRow = lastMemberRow + 1;
            }
            else
            {
                cursor.CurrentColumn = lastMemberColumn + 1;
            }
        }

        private static bool IsDefaultFormatNumber(ExcelRange range)
        {
            var numberFormats = new List<string> {range.Style.Numberformat.Format};

            if (!string.IsNullOrWhiteSpace(range.StyleName))
            {
                var namedStyle = range.Worksheet.Workbook.Styles.NamedStyles
                    .SingleOrDefault(r => r.Name == range.StyleName);

                if (namedStyle != null)
                {
                    numberFormats.Add(namedStyle.Style.Numberformat.Format);
                }
            }

            return numberFormats.TrueForAll(s => s == "General");
        }

        private void ApplyStyle(IExcelMemberOptions options, bool isHeaderRow, ExcelRange cells)
        {
            string styleName;
            ExcelStyle style;
            if (isHeaderRow)
            {
                styleName = options.HeaderStyleName;
                style = options.HeaderStyle;
            }
            else
            {
                styleName = options.StyleName;
                style = options.Style;
            }

            ExcelStyleApplier.ApplyStyle(styleName, style, cells);
        }

        private static int GetLastMemberPosition(int current, int count)
        {
            return current + count - 1;
        }
    }
}
