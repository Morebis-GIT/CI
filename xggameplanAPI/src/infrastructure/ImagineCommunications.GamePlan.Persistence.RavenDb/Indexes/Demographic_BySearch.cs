using System.Linq;
using ImagineCommunications.GamePlan.Domain.Shared.Demographics;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class Demographic_BySearch : AbstractIndexCreationTask<Demographic, Demographic_BySearch.IndexedFields>
    {
        public static string DefaultIndexName => "Demographic/BySearch";

        public class IndexedFields
        {
            public string ExternalRef { get; set; }
            public string Name { get; set; }
            public string ShortName { get; set; }
            public int DisplayOrder { get; set; }
            public bool Gameplan { get; set; }
        }

        public Demographic_BySearch()
        {
            var name = IndexName;
            Map = demographics =>
                from d in demographics
                select new
                {
                    d.ExternalRef,
                    d.Name,
                    d.ShortName,
                    d.DisplayOrder,
                    d.Gameplan
                };
            Index(d => d.ExternalRef, FieldIndexing.Analyzed);
        }
    }
}
