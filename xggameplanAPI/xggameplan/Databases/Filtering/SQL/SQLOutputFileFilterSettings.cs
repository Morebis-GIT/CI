using System;

namespace xggameplan.OutputFiles.Filtering.SQL
{
    /// <summary>
    /// Filter settings for SQL
    /// </summary>
    public class SQLOutputFileFilterSettings
    {
        public string OutputFileFolder { get; set; }
        public string SQL { get; set; }

        public string ResultsFile { get; set; }

        public Char Delimiter { get; set; }
    }
}