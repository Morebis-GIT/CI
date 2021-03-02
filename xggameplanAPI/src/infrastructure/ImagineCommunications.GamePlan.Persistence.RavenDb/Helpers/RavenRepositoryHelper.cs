using System.Collections.Generic;
using System.Linq;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Helpers
{
    public static class RavenRepositoryHelper
    {
        /// <summary>
        /// To avoid 'Exceeded maximum clause count in the query.' exception
        /// in case when elements in clause greater than 1000 
        /// we will group them for several request 
        /// </summary>
        /// <param name="elements">search terms to be grouped</param>
        /// <param name="maxClauseCount">maximum number of elements to be searched by raven</param>
        /// <returns>Grouped terms to be searched</returns>
        public static IEnumerable<IGrouping<int, T>> GroupElementsForInClause<T>(IEnumerable<T> elements, int maxClauseCount = 1000)
        {
            if (maxClauseCount == 0)
            {
                return Enumerable.Empty<IGrouping<int, T>>();
            }

            maxClauseCount = maxClauseCount < 0 ? 1000 : maxClauseCount;

            return elements.Select((x, i) => new { Item = x, Index = i })
                .GroupBy(x => x.Index / maxClauseCount, x => x.Item);
        }
    }
}
