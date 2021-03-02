using System.Collections.Generic;

namespace xggameplan.Reports.Models
{
    public class ExcelReportGrid
    {
        public int MaxColumnCount { get; set; }

        public List<ExcelReportRow> HeaderRows { get; set; }

        public List<ExcelReportRow> BodyRows { get; set; }

        public ExcelReportGrid()
        {
            HeaderRows = new List<ExcelReportRow>();
            BodyRows = new List<ExcelReportRow>();
        }

        public bool HasData
        {
            get
            {
                foreach (var row in BodyRows)
                {
                   if(row.HasData)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
