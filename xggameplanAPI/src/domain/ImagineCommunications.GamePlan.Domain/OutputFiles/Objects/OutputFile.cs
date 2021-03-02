using System.Collections.Generic;

namespace ImagineCommunications.GamePlan.Domain.OutputFiles.Objects
{
    /// <summary>
    /// Details of an output file
    /// </summary>
    public class OutputFile
    {
        /// <summary>
        /// File ID
        /// </summary>
        public string FileId { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Filename used by AutoBook. We have no control over this name.
        /// </summary>
        public string AutoBookFileName { get; set; }


        /// <summary>
        /// List of columns in the output file
        /// </summary>
        public List<OutputFileColumn> Columns = new List<OutputFileColumn>();

        /// <summary>
        /// Name of file as it appears in SQL query
        /// </summary>
        [Raven.Imports.Newtonsoft.Json.JsonIgnore]
        public string QueryFileName =>
            FileId.Replace(".OUT", ".TXT").Replace(".out", ".txt");
    }
}
