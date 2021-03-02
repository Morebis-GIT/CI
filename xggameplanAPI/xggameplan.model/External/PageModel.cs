using System.Collections.Generic;

namespace xggameplan.Model
{
    /// <summary>
    /// Page of items
    /// </summary>
    /// <typeparam name="T">Page item</typeparam>
    public class PageModel<T>
    {
        /// <summary>
        /// Items on page
        /// </summary>
        public List<T> Items { get; set; }

        /// <summary>
        /// Total number of items on all pages
        /// </summary>
        public int TotalItems { get; set; }
    }
}
