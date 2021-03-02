using System;
using System.Data;
using System.IO;

namespace xggameplan.common.Utilities
{
    /// <summary>
    /// CSV utilities
    /// </summary>
    public static class CSVUtilities
    {
        /// <summary>
        /// Returns DataTable for CSV, row range can be filtered
        /// </summary>
        /// <param name="file"></param>
        /// <param name="delimiter"></param>
        /// <param name="minRowIndex"></param>
        /// <param name="maxRowIndex"></param>
        /// <returns></returns>
        public static DataTable LoadCSVInToDataTable(string file, Char delimiter, int? minRowIndex, int? maxRowIndex)
        {
            var dataTable = new DataTable();

            using (var reader = new StreamReader(file))
            {
                int lines = 0;
                while (!reader.EndOfStream)
                {
                    lines++;
                    string[] values = reader.ReadLine().Split(delimiter);
                    if (lines == 1)
                    {
                        for (int index = 0; index < values.Length; index++)
                        {
                            _ = dataTable.Columns.Add(values[index], typeof(String));
                        }
                    }
                    else
                    {
                        int rowIndex = lines - 2;       // First line is zero
                        if ((minRowIndex == null) || (minRowIndex == -1) || (minRowIndex != -1 && rowIndex >= minRowIndex))    // Current row in min row range
                        {
                            if ((maxRowIndex == null) || (maxRowIndex == -1) || (maxRowIndex != -1 && rowIndex <= maxRowIndex))   // Current row in max row range
                            {
                                DataRow row = dataTable.NewRow();
                                for (int index = 0; index < values.Length; index++)
                                {
                                    row[index] = values[index];
                                }
                                dataTable.Rows.Add(row);
                            }
                        }
                    }
                }
            }

            return dataTable;
        }
    }
}
