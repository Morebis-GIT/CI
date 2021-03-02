using System.Linq;
using ImagineCommunications.GamePlan.Domain.LibrarySalesAreaPassPriorities;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;


namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class LibrarySalesAreaPassPriority_ByNameAndUidSortByUid
        : AbstractIndexCreationTask<LibrarySalesAreaPassPriority>
    {
        public static string DefaultIndexName => "LibrarySalesAreaPassPriority/ByNameAndUidSortByUid";

        public LibrarySalesAreaPassPriority_ByNameAndUidSortByUid()
        {
            Map = documents =>
                from doc in documents
                select new
                {
                    doc.Name,
                    doc.Uid
                };

            IndexSortOptionsStrings.Add("Uid", SortOptions.String);
        }
    }
}
