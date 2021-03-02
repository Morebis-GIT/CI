using System.Linq;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using Raven.Client.Indexes;
using Raven.Client.Linq.Indexing;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class Scenarios_MinimalData : AbstractIndexCreationTask<Scenario>
    {
        public static string DefaultIndexName => "Scenarios/Minimal";

        public Scenarios_MinimalData() => Map = scenarios =>
                                            from scenario in scenarios
                                            select new
                                            {
                                                scenario.Id,
                                                scenario.Name,
                                                scenario.DateModified,
                                                IsLibraried = scenario.IsLibraried.Boost(10),
                                                scenario.Passes
                                            };
    }
}