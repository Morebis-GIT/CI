using System.Collections.Generic;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.MemberConfig;
using OfficeOpenXml;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel.SheetBuilder.BlockBuilder
{
  public class BlockBuilder: IBlockBuilder
    {
        protected readonly List<IBlockOptionsBuilder> BlockOptionsBuilders = new List<IBlockOptionsBuilder>();
        public ExcelWorksheet Worksheet { get; }

        public BlockBuilder(ExcelWorksheet worksheet)
        {
            Worksheet = worksheet;
        }

        public IBlockOptionsBuilder Add<T>(T value)
        {
            var builder = new BlockOptionsBuilder<T>(value);
            BlockOptionsBuilders.Add(builder);
            return builder;
        }

        public void Write(IExcelDataWriter writer)
        {
            foreach (IBlockOptionsBuilder blockOptionsBuilder in BlockOptionsBuilders)
            {
                var options = blockOptionsBuilder.Build();

                var memberOptions = new ExcelMemberOptions
                {
                     Style = options.Style,
                     StyleName = options.StyleName,
                     ColSpan = options.ColSpan,
                     RowSpan = options.RowSpan,
                     HAlign = options.HAlign,
                     VAlign = options.VAlign,
                     BackgroundColor = options.BackgroundColor,
                     Merge = true
                };

                writer.WriteValue(options.Value, memberOptions, options.Display);
            }
        }

        public int GetRowCount()
        {
            var maxRowCount = 1;
            var currentRowCount = 0;
            var previousIsBlock = false;
            foreach (IBlockOptionsBuilder blockOptionsBuilder in BlockOptionsBuilders)
            {
                if (!previousIsBlock)
                {
                    currentRowCount = 0;
                }
                var options = blockOptionsBuilder.Build();

                switch (options.Display)
                {
                    case DisplayType.Block:
                        previousIsBlock = true;
                        break;
                    default:
                        previousIsBlock = false;
                        break;
                }
                currentRowCount += options.RowSpan;

                if (maxRowCount < currentRowCount)
                {
                    maxRowCount = currentRowCount;
                }
            }

            return maxRowCount;
        }
    }
}
