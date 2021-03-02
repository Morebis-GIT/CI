using System;
using System.Collections.Generic;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.MemberConfig;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.OneTable.EntityConfig;
using ImagineCommunications.GamePlan.ReportSystem.ReportConfiguration.MemberConfig;
using OfficeOpenXml;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel.SheetBuilder.OneTable
{
    public class OneTableSheetBuilder
        : BaseSheetBuilder<OneTableSheetBuilder>
        , IOneTableSheetBuilder
    {
        public OneTableSheetBuilder(IExcelStyleApplier styleApplier, ExcelWorksheet worksheet)
            : base(styleApplier, worksheet)
        {
        }

        public virtual void DataContent<T>(IEnumerable<T> data, IOneTableExcelReportConfiguration configuration)
            where T : class
        {
            SetSheetSettings(configuration);

            var baseConfiguration = ConvertToExcelReportConfiguration<T>(configuration);

            DataContentInternal(data, baseConfiguration);
        }

        public virtual void DataContent<T>(IEnumerable<T> data)
            where T : class
        {
            DataContentInternal(data);
        }

        private static ExcelReportConfiguration.EntityConfig.ExcelReportConfiguration ConvertToExcelReportConfiguration<T>(
            IOneTableExcelReportConfiguration configuration) where T : class
        {
            var members = new MemberConfigurationDictionary<ExcelMemberOptions>();

            foreach (var options in configuration.MemberConfigurations)
            {
                members.Add(options.Key, options.Value);
            }

            var baseConfiguration =
                new ExcelReportConfiguration.EntityConfig.ExcelReportConfiguration(configuration.Options, members);
            return baseConfiguration;
        }

        private void SetSheetSettings(IOneTableExcelReportConfiguration configuration)
        {
            var memberOptions = configuration.MemberConfigurations.GetActiveOrderlyOptions();
            var columnNumber = 1;
            foreach (var oneTableExcelMemberOptions in memberOptions)
            {
                Column(columnNumber, builder =>
                {
                    if (oneTableExcelMemberOptions.Width > 0)
                    {
                        builder.Width(oneTableExcelMemberOptions.Width);
                    }

                    if (oneTableExcelMemberOptions.IsAutoFitColumn)
                    {
                        builder.AutoFitColumn();
                    }

                    if (string.IsNullOrWhiteSpace(oneTableExcelMemberOptions.Format))
                    {
                        builder.Format(oneTableExcelMemberOptions.Format);
                    }
                });
                columnNumber++;
            }
        }

        private void Column(int columnNumber, Action<IColumnBuilder> action)
        {
            var column = Worksheet.Column(columnNumber);
            var columnBuilder = new ColumnBuilder(column);
            action(columnBuilder);
        }
    }
}
