using System.Collections.Generic;
using System.IO;
using ImagineCommunications.GamePlan.ReportSystem.Csv.CsvReportConfiguration.EntityConfig;

namespace ImagineCommunications.GamePlan.ReportSystem.Csv
{
    public interface ICsvReportBuilder
    {
        ICsvReportBuilder DataContent<T>(IEnumerable<T> data, ICsvReportConfiguration configuration)
            where T : class;
        ICsvReportBuilder DataContent<T>(IEnumerable<T> data)
            where T : class;
        ICsvReportBuilder Delimiter(string delimiter);
        ICsvReportBuilder Comments(params string[] comments);

        void SaveAs(TextWriter writer);
        string GetAsString();
    }
}
