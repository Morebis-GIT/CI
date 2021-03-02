using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.EntityConfig;
using ImagineCommunications.GamePlan.ReportSystem.Excel.SheetBuilder;
using xggameplan.Model;

namespace xggameplan.Reports.Common
{
    public class ReportColumnFormatter : IReportColumnFormatter
    {
        public List<T> ApplySettings<T>(IEnumerable<T> data, IEnumerable<ColumnStatusModel> columnStatusModelList,
            ExcelConfigurationBuilder<T> configurationBuilder) where T : class
        {
            var orderData = data?.ToList() ?? new List<T>();
            if (!orderData.Any()
                || columnStatusModelList is null
                || !columnStatusModelList.Any() || configurationBuilder == null)
            {
                return orderData;
            }

            var sortFields = new List<string>();

            foreach (var columnStatusModel in columnStatusModelList)
            {
                if (columnStatusModel.Ignore)
                {
                    configurationBuilder.ForMember(columnStatusModel.Name, o => o.Ignore());
                }
                else
                {
                    configurationBuilder.ForMember(columnStatusModel.Name, o => o.Order(columnStatusModel.Order));

                    if (String.Equals(columnStatusModel.SortDirection, "ASC", StringComparison.OrdinalIgnoreCase)
                        || String.Equals(columnStatusModel.SortDirection, "DESC", StringComparison.OrdinalIgnoreCase))
                    {
                        sortFields.Add(columnStatusModel.Name + " " + columnStatusModel.SortDirection);
                    }
                }
            }

            if (sortFields.Any())
            {
                orderData = data.AsQueryable().OrderBy(String.Join(",", sortFields)).ToList();
            }

            return orderData;
        }

        public void AutoFitAll(ISheetBuilder sheetBuilder) => AutoFit(AllColumns, sheetBuilder);

        public void AutoFit(List<int> columns, ISheetBuilder sheetBuilder) =>
            columns?.ForEach(column => sheetBuilder.Column(column, columnBuilder => columnBuilder.AutoFitColumn()));

        private List<int> AllColumns => Enumerable.Range(1, GetCounts()).Select(x => x++).ToList();

        private int GetCounts() => typeof(CampaignReportModel).GetProperties().Length;
    }
}
