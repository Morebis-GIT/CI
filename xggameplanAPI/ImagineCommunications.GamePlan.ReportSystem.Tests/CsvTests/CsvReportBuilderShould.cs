using System;
using System.Collections.Generic;
using System.Text;
using ImagineCommunications.GamePlan.ReportSystem.Csv;
using ImagineCommunications.GamePlan.ReportSystem.Csv.CsvReportConfiguration.EntityConfig;
using ImagineCommunications.GamePlan.ReportSystem.Tests.SampleData;
using Xunit;

namespace ImagineCommunications.GamePlan.ReportSystem.Tests.CsvTests
{
    public class CsvReportBuilderShould
    {
        [Fact]
        public void GetAsString()
        {
            var salesData = GetSaleData();

            var reportBuilder = GetCsvReportBuilder(salesData);

            var csvData = reportBuilder.GetAsString();

            Assert.Equal(GetExpectedResult(salesData), csvData);
        }

        [Fact]
        public void GetAsByteArray()
        {
            var salesData = GetSaleData();

            var reportBuilder = GetCsvReportBuilder(salesData);

            var csvData = reportBuilder.GetAsByteArray();

            var expected = GetExpectedResult(salesData);

            Assert.Equal(Encoding.ASCII.GetBytes(expected), csvData);
        }

        [Fact]
        public void WriteComment()
        {
            var reportBuilder = new CsvReportBuilder()
                .Comments("Test Line 1", "Test Line 2");

            var csvData = reportBuilder.GetAsString();
            var sb = new StringBuilder();
            _ = sb.AppendLine("#Test Line 1");
            _ = sb.AppendLine("#Test Line 2");

            Assert.Equal(sb.ToString(), csvData);
        }

        private static string GetExpectedResult(List<SaleData> salesData)
        {
            var sb = new StringBuilder();
            _ = sb.AppendLine("id,region,country,city,amount,date");
            foreach (var saleData in salesData)
            {
                _ = sb.AppendLine(
                    $"{saleData.id},{saleData.region},{saleData.country},{saleData.city},{saleData.amount},{saleData.date.ToShortDateString()}");
            }
            return sb.ToString();
        }

        private static CsvReportBuilder GetCsvReportBuilder(List<SaleData> salesData)
        {
            var dataBuilder = new CsvConfigurationBuilder<SaleData>()
                .ForMember(m => m.date, opts => opts.Formatter(o => ((DateTime)o).ToShortDateString()));

            var reportBuilder = new CsvReportBuilder();
            _ = reportBuilder.DataContent(salesData, dataBuilder.BuildConfiguration());
            return reportBuilder;
        }

        private static List<SaleData> GetSaleData()
        {
            var saleData = new SaleData
            {
                id = 1,
                amount = 10,
                city = "London",
                country = "UK",
                date = new DateTime(2019, 1, 1),
                region = "UK"
            };
            var salesData = new List<SaleData>
            {
                saleData
            };

            return salesData;
        }
    }
}
