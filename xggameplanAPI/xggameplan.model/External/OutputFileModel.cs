using System.Collections.Generic;

namespace xggameplan.Model
{
    public class OutputFileModel
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
        /// List of columns in the output file
        /// </summary>
        public List<OutputFileColumnModel> Columns = new List<OutputFileColumnModel>();
    }


}
