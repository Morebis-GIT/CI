using System.Collections.Generic;
using System.Linq;

namespace xggameplan.common.Utilities
{
    /// <summary>
    /// List utilities
    /// </summary>
    public static class ListUtilities
    {
        /// <summary>
        /// Filters subset of items for specified page number based on specified
        /// number of items per page
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">List to filter</param>
        /// <param name="pageItems">Number of items per page</param>
        /// <param name="pageNo">Page number to return (First page is zero)</param>
        /// <returns></returns>
        public static List<T> GetPageItems<T>(List<T> items, int pageItems, int pageNo)
        {
            return items.Skip(pageNo * pageItems).Take(pageItems).ToList();
        }
    }
}
