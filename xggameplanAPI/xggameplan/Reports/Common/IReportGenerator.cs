using System.Collections.Generic;
using System.IO;
using xggameplan.Model;
using xggameplan.Reports.Common;

namespace xggameplan.Reports
{
    public interface IReportGenerator<T> where T : class
    {
        byte[] GetReportAsByteArray(string sheetName, IEnumerable<T> data);

        byte[] GetReportAsByteArray(string sheetName, IEnumerable<T> data,
            IEnumerable<ColumnStatusModel> columnStatusList, IReportColumnFormatter reportColumnHelper);

        Stream GetReportAsStream(string sheetName, IEnumerable<T> data);

        Stream GetReportAsStream(string sheetName, IEnumerable<T> data,
            IEnumerable<ColumnStatusModel> columnStatusList, IReportColumnFormatter reportColumnHelper);
    }
}
