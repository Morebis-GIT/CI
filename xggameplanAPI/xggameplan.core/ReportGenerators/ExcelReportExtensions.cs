using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.OneTable.EntityConfig;

namespace xggameplan.core.ReportGenerators
{
    public static class ExcelReportExtensions
    {
        public static OneTableExcelConfigurationBuilder<TEntity> SetDefaultStyles<TEntity>(this OneTableExcelConfigurationBuilder<TEntity> builder)
            where TEntity : class
        {
            builder.DefaultStyle(GamePlanReportStyles.DataCellStyle.Name)
                .HeaderStyle(GamePlanReportStyles.HeaderStyle.Name)
                .AlternateBackgroundColors(GamePlanReportStyles.AlternateBackgroundColors);

            return builder;
        }
    }
}
