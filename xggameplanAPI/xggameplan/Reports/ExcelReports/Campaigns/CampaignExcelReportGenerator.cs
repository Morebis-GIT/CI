using System;
using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.ReportSystem.Excel;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.EntityConfig;
using xggameplan.core.ReportGenerators;
using xggameplan.core.ReportGenerators.ReportFormatters;
using xggameplan.Model;
using xggameplan.Reports.Common;

namespace xggameplan.Reports.ExcelReports.Campaigns
{
    public class CampaignExcelReportGenerator : ICampaignExcelReportGenerator
    {
        public byte[] GetReportAsByteArray(string sheetName,
            IEnumerable<CampaignReportModel> data,
            IEnumerable<ColumnStatusModel> columnStatusList,
            IReportColumnFormatter reportColumnHelper)
        {
            var confBuilder = CreateExcelConfigurationBuilder();

            using (var reportBuilder = new ExcelReportBuilder(new ExcelStyleApplier())
                .PredefineStyles(GamePlanReportStyles.AllPredefineStyles))
            {
                var orderedData = reportColumnHelper.ApplySettings(data, columnStatusList, confBuilder);

                reportBuilder.Sheet(sheetName, sheetBuilder =>
                {
                    sheetBuilder.DataContent(orderedData, confBuilder.BuildConfiguration());
                    reportColumnHelper.AutoFitAll(sheetBuilder);
                });
                return reportBuilder.Save();
            }
        }

        public byte[] GetReportAsByteArray(string sheetName, IEnumerable<CampaignReportModel> data)
            => throw new NotImplementedException();

        public Stream GetReportAsStream(string sheetName, IEnumerable<CampaignReportModel> data)
            => throw new NotImplementedException();

        public Stream GetReportAsStream(string sheetName, IEnumerable<CampaignReportModel> data,
            IEnumerable<ColumnStatusModel> columnStatusList, IReportColumnFormatter reportColumnHelper)
            => throw new NotImplementedException();

        /// <summary>
        /// Creates campaign list excel configuration builder.
        /// </summary>
        /// <param name="builder">The builder.</param>
        private static ExcelConfigurationBuilder<CampaignReportModel> CreateExcelConfigurationBuilder()
        {
            var confBuilder = new ExcelConfigurationBuilder<CampaignReportModel>()
                .DefaultStyle(GamePlanReportStyles.DataCellStyle.Name)
                .HeaderStyle(GamePlanReportStyles.HeaderStyle.Name)
                .AlternateBackgroundColors(GamePlanReportStyles.AlternateBackgroundColors);

            var currencyFormatter = new CurrencyReportFormatter();

            confBuilder
                .ForMember(с => с.DemoGraphic, o => o.Header("Demographic"))
                .ForMember(с => с.Payback, o => o.Formatter(currencyFormatter))
                .ForMember(с => с.RevenueBooked, o => o.Formatter(currencyFormatter))
                .ForMember(с => с.TopTail, o => o.Header("Top/Tail"))
                .ForMember(с => с.RatingsDifferenceExcludingPayback,
                    o => o.Formatter(ReportFormatter.DecimalRoundingFormatter).Header("+/- Ratings Excl. PB"))
                .ForMember(с => с.ValueDifference, o => o.Formatter(currencyFormatter).Header("+/- Value"))
                .ForMember(с => с.ValueDifferenceExcludingPayback, o => o.Formatter(currencyFormatter).Header("+/- Value Excl. PB"))
                .ForMember(с => с.AchievedPercentageTargetRatings,
                    o => o.Formatter(ReportFormatter.DecimalRoundingFormatter).Header("Achieved % (ratings)"))
                .ForMember(с => с.AchievedPercentageRevenueBudget,
                    o => o.Formatter(ReportFormatter.DecimalRoundingFormatter).Header("Achieved % (value)"))
                .ForMember(с => с.Spots, o => o.Header("Spots (actual)"))
                .ForMember(с => с.CreationDate, o => o.Formatter(ReportFormatter.ConvertToShortDate))
                .ForMember(с => с.ProductAssignee, o => o.Header("Person (Product Assignee)"))
                .ForMember(с => с.MediaSalesGroup, o => o.Header("Media Group"));

            return confBuilder;
        }
    }
}
