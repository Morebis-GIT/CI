using System.Linq;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class Scenarios_Default
        : AbstractIndexCreationTask<Scenario>
    {
        public static string DefaultIndexName => "Scenarios/Default";

        public Scenarios_Default()
        {
            Map = scenarios =>
                from scenario in scenarios
                select new
                {
                    scenario.Id,
                    scenario.Name,
                    scenario.DateCreated,
                    scenario.DateModified,
                    scenario.CampaignPassPriorities,
                    scenario.Passes
                };

            Index(r => r.Name, FieldIndexing.Analyzed);
        }
    }
}
