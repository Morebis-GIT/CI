using System.Collections.Generic;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.EntityConfig;
using ImagineCommunications.GamePlan.ReportSystem.Excel.ExcelReportConfiguration.MemberConfig;

namespace ImagineCommunications.GamePlan.ReportSystem.Excel
{
    public interface IExcelDataWriter
    {
        void WriteValue<T>(T value, IExcelMemberOptions options, DisplayType display);
        void Write<TEntity>(IEnumerable<TEntity> source, IExcelReportConfiguration configuration) where TEntity : class;
    }
}
