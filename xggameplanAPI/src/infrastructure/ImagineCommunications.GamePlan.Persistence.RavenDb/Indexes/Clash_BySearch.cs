using System.Linq;
using ImagineCommunications.GamePlan.Domain.BusinessRules.Clashes.Objects;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class Clash_BySearch : AbstractIndexCreationTask<Clash, Clash_BySearch.IndexedFields>
    {
        public static string DefaultIndexName => "Clash/BySearch";

        public class IndexedFields
        {
            public string TokenizedName { get; set; }

            public string Externalref { get; set; }

            public string Description { get; set; }
        }

        public Clash_BySearch()
        {
            var name = IndexName;
            Map = clashes => from clash in clashes
                             select new
                             {
                                 TokenizedName = $"{clash.Externalref} {clash.Description}",
                                 clash.Externalref,
                                 clash.Description
                             };

            Index(c => c.TokenizedName, FieldIndexing.Analyzed);
        }
    }
}
