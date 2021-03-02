namespace xggameplan.Model
{
    public class ColumnStatusModel
    {
        /// <summary>
        /// Name
        /// Column Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Ignore
        /// Is Column ignored, if it's, will ignore from response
        /// </summary>
        public bool Ignore { get; set; }

        /// <summary>
        /// Sort
        /// Is Column sorted, if it's, will return as sorted
        /// </summary>
        public bool Sort { get; set; }

        /// <summary>
        /// SortDirection
        /// If column sorted, needs to define sort direction as ASC or DESC
        /// </summary>
        public string SortDirection { get; set; }

        /// <summary>
        /// Order
        /// Exact position of column on the grid as integer, all columns order need to define as int
        /// </summary>
        public int Order { get; set; }
    }
}
