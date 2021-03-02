namespace xggameplan.Model
{
    /// <summary>
    /// Summary item for runs filter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FilterRunsSummaryItemModel<T>
    {
        /// <summary>
        /// Item ID
        /// </summary>
        public T Id { get; set; }

        /// <summary>
        /// Item name
        /// </summary>
        public string Name { get; set; }
    }
}
