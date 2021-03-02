using System.Globalization;
using System.Linq;
using ImagineCommunications.GamePlan.Domain.Passes.Objects;
using ImagineCommunications.GamePlan.Domain.Runs.Objects;
using ImagineCommunications.GamePlan.Domain.Scenarios.Objects;
using ImagineCommunications.GamePlan.Persistence.RavenDb.Reducers;
using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

namespace ImagineCommunications.GamePlan.Persistence.RavenDb.Indexes
{
    public class Runs_BySearch : AbstractIndexCreationTask<Run, RunReducedResult>
    {
        public static string DefaultIndexName => "Runs/BySearch";
        public const string DashReplacer = "_dash_";

        public Runs_BySearch()
        {
            Map = runs => from run in runs
                          let scenarios = LoadDocument<Scenario>(run.Scenarios.Select(s => "scenarios/" + s.Id))
                          let passes = scenarios.SelectMany(s => s.Passes)
                                           .Select(p => LoadDocument<Pass>("passes/" + p.Id))
                          select new
                          {
                              Query = new object[]
                              {
                                run.Id.ToString().Substring(run.Id.ToString().IndexOf('/') + 1).Replace("-", DashReplacer),
                                run.Description.Replace("-", DashReplacer),
                                run.Author.Name.Replace("-", DashReplacer),
                                run.Scenarios.Select(s => s.Id.ToString().Replace("-", DashReplacer)).ToArray(),
                                scenarios.Select(s => s.Name.ToString(CultureInfo.InvariantCulture).Replace("-", DashReplacer)).ToArray(),
                                passes.Select(p => p.Id.ToString(CultureInfo.InvariantCulture).Substring(p.Id.ToString(CultureInfo.InvariantCulture).IndexOf('/') + 1)).ToArray(),
                                passes.Select(p => p.Name.ToString(CultureInfo.InvariantCulture).Replace("-", DashReplacer)).ToArray()
                              },
                              AuthorId = run.Author.Id,
                              AuthorName = run.Author.Name,
                              run.RunStatus,
                              run.StartDate,
                              run.EndDate,
                              ExecuteStartedDateTime = run.ExecuteStartedDateTime == null ? run.CreatedDateTime : run.ExecuteStartedDateTime,
                              run.Description
                          };

            Index(r => r.Query, FieldIndexing.Analyzed);
        }
    }
}
