using System.Collections.Generic;

namespace xggameplan.Reports.Models
{
    public class ExcelReportRow
    {
        public List<ExcelReportCell> Cells { get; set; }

        public ExcelReportRow()
        {
            Cells = new List<ExcelReportCell>();
        }

        public bool HasData
        {
            get
            {
                if (Cells.Count > 1)
                {
                    for (var i = 1; i < Cells.Count; i++)
                    {
                        if (Cells[i].HasData)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
    }
}
